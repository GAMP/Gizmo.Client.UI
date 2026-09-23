#!/usr/bin/env python3
"""
Grafit avatar proxy.

Runs on the SAME machine as GizmoService.exe. The client shell (running in
every station's WebView, loaded as a skin from the Gizmo Server) has no
credentials that are allowed to touch the management REST API
(/api/v3/users/{id}/picture) - that endpoint is gated behind an operator
account, the same kind Gizmo Manager logs in with. This proxy holds that
one operator credential in a local config file and exposes two dead-simple
endpoints for the shell to call instead:

    GET  /avatar/{userId}   -> raw image bytes, or 404 if the user has none
    POST /avatar/{userId}   -> body = raw (already-compressed) image bytes,
                                Content-Type = image/webp or image/jpeg

It never runs arbitrary code from the client, never trusts a station to
say who it's acting as beyond the numeric id already in the URL (the shell
only ever asks for its own logged-in user's id), and never exposes the
operator credential or the Gizmo bearer token to the browser.

It also keeps a small local index (avatar_index.json) of which user ids
have an avatar set - self-populated from ordinary GET/POST traffic, no
separate backfill job needed - so a third endpoint can decide "should we
nudge this user to add a photo" as a pure in-memory dict lookup, with zero
extra round-trips to the real Gizmo API:

    GET  /avatar/{userId}/nudge   -> {"show": true|false}

Zero third-party dependencies - only the Python standard library - so
there is nothing to `pip install` on the server. Python 3.8+.
"""

import base64
import json
import logging
import os
import sys
import threading
import time
import urllib.error
import urllib.request
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer

CONFIG_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "config.json")
LOG_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "avatar_proxy.log")
INDEX_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "avatar_index.json")

# Printed on startup so a log always says which build is actually running -
# a replaced-but-not-restarted proxy is otherwise very hard to spot (the
# traceback shows the old code's line numbers against the new file's text).
PROXY_VERSION = "2026-07-28.2"

MAX_UPLOAD_BYTES = 2 * 1024 * 1024  # 2 MB hard ceiling; the client compresses to tens of KB
TOKEN_REFRESH_MARGIN_SECONDS = 60   # re-auth this long before the cached token's own exp

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(message)s",
    handlers=[logging.FileHandler(LOG_PATH, encoding="utf-8"), logging.StreamHandler(sys.stdout)],
)
log = logging.getLogger("avatar_proxy")


def load_config():
    with open(CONFIG_PATH, "r", encoding="utf-8") as f:
        cfg = json.load(f)
    required = ["gizmo_api_base", "operator_username", "operator_password", "listen_port"]
    missing = [k for k in required if not cfg.get(k) and cfg.get(k) != 0]
    if missing:
        raise RuntimeError(f"config.json is missing required keys: {', '.join(missing)}")
    cfg.setdefault("branch_id", None)
    cfg.setdefault("register_id", None)
    cfg.setdefault("listen_host", "0.0.0.0")
    cfg.setdefault("allowed_origin", "*")
    cfg.setdefault("nudge_interval_logins", 5)
    return cfg


CONFIG = load_config()


# ─────────────────────────── avatar-existence index ─────────────────────────
# One JSON object, one entry per user id ever seen. At ~5k accounts this is a
# few hundred KB at most - loaded once into memory, so every lookup after
# that is an O(1) dict access instead of a network round-trip to Gizmo.
#
#   {"12345": {"hasAvatar": true},
#    "67890": {"hasAvatar": false, "loginCount": 3, "lastNudgeAtLogin": 1}}

class AvatarIndex:
    def __init__(self, path):
        self._path = path
        self._lock = threading.Lock()
        self._data = self._load()

    def _load(self):
        try:
            with open(self._path, "r", encoding="utf-8") as f:
                return json.load(f)
        except (FileNotFoundError, json.JSONDecodeError):
            return {}

    def _save(self):
        # Write-to-temp-then-rename so a crash mid-write can never leave a
        # half-written, corrupt index behind.
        tmp_path = self._path + ".tmp"
        with open(tmp_path, "w", encoding="utf-8") as f:
            json.dump(self._data, f)
        os.replace(tmp_path, self._path)

    def set_has_avatar(self, user_id, has_avatar):
        key = str(user_id)
        with self._lock:
            entry = self._data.setdefault(key, {})
            if entry.get("hasAvatar") == has_avatar:
                return
            entry["hasAvatar"] = has_avatar
            self._save()

    def should_show_nudge(self, user_id, interval_logins):
        key = str(user_id)
        with self._lock:
            entry = self._data.setdefault(key, {})

            if entry.get("hasAvatar"):
                return False

            login_count = entry.get("loginCount", 0) + 1
            entry["loginCount"] = login_count

            last_shown = entry.get("lastNudgeAtLogin")
            show = last_shown is None or (login_count - last_shown) >= interval_logins

            if show:
                entry["lastNudgeAtLogin"] = login_count

            self._save()
            return show


AVATAR_INDEX = AvatarIndex(INDEX_PATH)


# ─────────────────────────── JSON field lookup ───────────────────────────

def json_get(obj, *names):
    """Case-insensitive field lookup, also looking one level inside a
    common wrapper key.

    The Gizmo API is not consistent about JSON casing: its own client
    library deserializes payloads with a bare `new JsonSerializerOptions()`
    (PascalCase, case-sensitive), so payload models come back as "Token" /
    "Picture", while the error-handling middleware emits camelCase
    ("httpStatusCode", "errorCode"). Pinning one casing is how the proxy
    ended up raising KeyError: 'token' against a perfectly good 200
    response.
    """
    if not isinstance(obj, dict):
        return None

    wanted = {n.lower() for n in names}

    for key, value in obj.items():
        if key.lower() in wanted:
            return value

    for wrapper in ("result", "data", "value"):
        for key, value in obj.items():
            if key.lower() == wrapper and isinstance(value, dict):
                found = json_get(value, *names)
                if found is not None:
                    return found

    return None


# ─────────────────────────── operator token cache ───────────────────────────

class TokenCache:
    def __init__(self):
        self._lock = threading.Lock()
        self._token = None
        self._expires_at = 0

    def get(self, force_refresh=False):
        with self._lock:
            if force_refresh or self._token is None or time.time() >= self._expires_at:
                self._token, self._expires_at = self._fetch_new_token()
            return self._token

    @staticmethod
    def _decode_jwt_exp(token):
        try:
            payload_b64 = token.split(".")[1]
            padding = "=" * (-len(payload_b64) % 4)
            payload = json.loads(base64.urlsafe_b64decode(payload_b64 + padding))
            return payload.get("exp")
        except Exception:
            return None

    def _fetch_new_token(self):
        params = {
            "Username": CONFIG["operator_username"],
            "Password": CONFIG["operator_password"],
        }
        if CONFIG.get("branch_id") is not None:
            params["BranchId"] = CONFIG["branch_id"]
        if CONFIG.get("register_id") is not None:
            params["RegisterId"] = CONFIG["register_id"]

        query = "&".join(f"{k}={urllib.request.quote(str(v))}" for k, v in params.items())
        url = f"{CONFIG['gizmo_api_base']}/auth/accesstoken?{query}"

        req = urllib.request.Request(url, method="GET", headers={"Accept": "application/json"})
        try:
            with urllib.request.urlopen(req, timeout=10) as resp:
                data = json.loads(resp.read().decode("utf-8"))
        except urllib.error.HTTPError as e:
            body = e.read().decode("utf-8", errors="replace")
            log.error("Operator auth failed: HTTP %s %s", e.code, body)
            raise

        token = json_get(data, "token", "accessToken")
        if not isinstance(token, str) or not token:
            # Keys only, never the body - whatever token IS in there under
            # a name we do not know must not end up in a log file.
            keys = sorted(data.keys()) if isinstance(data, dict) else type(data).__name__
            log.error("Auth succeeded but no token field found in response. Fields: %s", keys)
            raise RuntimeError("Gizmo auth response contained no token field")

        exp = self._decode_jwt_exp(token) or (time.time() + 1500)
        expires_at = exp - TOKEN_REFRESH_MARGIN_SECONDS
        log.info("Obtained new operator token, valid until %s", time.ctime(exp))
        return token, expires_at


TOKENS = TokenCache()


# ─────────────────────────── Gizmo API calls ───────────────────────────

def gizmo_request(method, path, body_obj=None, retry_on_401=True):
    token = TOKENS.get()
    url = f"{CONFIG['gizmo_api_base']}{path}"
    data = json.dumps(body_obj).encode("utf-8") if body_obj is not None else None
    headers = {"Authorization": f"Bearer {token}", "Accept": "application/json"}
    if data is not None:
        headers["Content-Type"] = "application/json"

    req = urllib.request.Request(url, data=data, method=method, headers=headers)
    try:
        with urllib.request.urlopen(req, timeout=15) as resp:
            raw = resp.read()
            return resp.status, (json.loads(raw.decode("utf-8")) if raw else None)
    except urllib.error.HTTPError as e:
        if e.code == 401 and retry_on_401:
            log.warning("Gizmo API returned 401, forcing token refresh and retrying once")
            TOKENS.get(force_refresh=True)
            return gizmo_request(method, path, body_obj, retry_on_401=False)
        raw = e.read()
        try:
            parsed = json.loads(raw.decode("utf-8"))
        except Exception:
            parsed = {"raw": raw.decode("utf-8", errors="replace")}
        return e.code, parsed


def sniff_content_type(raw: bytes) -> str:
    if raw.startswith(b"\xff\xd8\xff"):
        return "image/jpeg"
    if raw.startswith(b"\x89PNG\r\n\x1a\n"):
        return "image/png"
    if raw[0:4] == b"RIFF" and raw[8:12] == b"WEBP":
        return "image/webp"
    if raw.startswith(b"GIF87a") or raw.startswith(b"GIF89a"):
        return "image/gif"
    return "application/octet-stream"


# ─────────────────────────── HTTP handler ───────────────────────────

class AvatarProxyHandler(BaseHTTPRequestHandler):
    server_version = "GrafitAvatarProxy/1.0"

    def log_message(self, fmt, *args):
        log.info("%s - %s", self.address_string(), fmt % args)

    def _cors_headers(self):
        self.send_header("Access-Control-Allow-Origin", CONFIG["allowed_origin"])
        self.send_header("Access-Control-Allow-Methods", "GET, POST, OPTIONS")
        self.send_header("Access-Control-Allow-Headers", "Content-Type")
        self.send_header("Access-Control-Max-Age", "600")

    def _send_json(self, status, obj):
        body = json.dumps(obj).encode("utf-8")
        self.send_response(status)
        self._cors_headers()
        self.send_header("Content-Type", "application/json; charset=utf-8")
        self.send_header("Content-Length", str(len(body)))
        self.end_headers()
        self.wfile.write(body)

    def _send_bytes(self, status, raw, content_type):
        self.send_response(status)
        self._cors_headers()
        self.send_header("Content-Type", content_type)
        self.send_header("Content-Length", str(len(raw)))
        self.send_header("Cache-Control", "no-store")
        self.end_headers()
        self.wfile.write(raw)

    def do_OPTIONS(self):
        self.send_response(204)
        self._cors_headers()
        self.end_headers()

    def _parse_avatar_path(self):
        """Returns (user_id, suffix) for /avatar/{id} or /avatar/{id}/nudge,
        or (None, None) if the path doesn't match either shape."""
        parts = [p for p in self.path.split("?")[0].split("/") if p]
        if len(parts) >= 2 and parts[0] == "avatar" and parts[1].lstrip("-").isdigit():
            suffix = parts[2] if len(parts) == 3 else None
            if len(parts) > 3:
                return None, None
            return int(parts[1]), suffix
        return None, None

    def do_GET(self):
        if self.path == "/health":
            self._send_json(200, {"status": "ok"})
            return

        user_id, suffix = self._parse_avatar_path()
        if user_id is None:
            self._send_json(404, {"error": "not_found"})
            return

        if suffix == "nudge":
            show = AVATAR_INDEX.should_show_nudge(user_id, CONFIG["nudge_interval_logins"])
            self._send_json(200, {"show": show})
            return

        if suffix is not None:
            self._send_json(404, {"error": "not_found"})
            return

        try:
            status, data = gizmo_request("GET", f"/users/{user_id}/picture")
        except Exception as ex:
            log.exception("GET avatar failed for user %s", user_id)
            self._send_json(502, {"error": "upstream_unreachable", "detail": str(ex)})
            return

        picture_b64 = json_get(data, "picture") if data else None

        if status != 200 or not picture_b64:
            AVATAR_INDEX.set_has_avatar(user_id, False)
            self._send_json(404, {"error": "no_picture"})
            return

        try:
            raw = base64.b64decode(picture_b64)
        except Exception:
            self._send_json(500, {"error": "decode_failed"})
            return

        AVATAR_INDEX.set_has_avatar(user_id, True)
        self._send_bytes(200, raw, sniff_content_type(raw))

    def do_POST(self):
        user_id, suffix = self._parse_avatar_path()
        if user_id is None or suffix is not None:
            self._send_json(404, {"error": "not_found"})
            return

        length = int(self.headers.get("Content-Length", 0))
        if length <= 0:
            self._send_json(400, {"error": "empty_body"})
            return
        if length > MAX_UPLOAD_BYTES:
            self._send_json(413, {"error": "too_large", "max_bytes": MAX_UPLOAD_BYTES})
            return

        raw = self.rfile.read(length)
        picture_b64 = base64.b64encode(raw).decode("ascii")

        try:
            # PascalCase to match UserModelPicture.Picture: the API
            # deserializes with case-sensitive default options, so a
            # lowercase "picture" key binds to nothing and the upload
            # silently saves an empty image.
            status, data = gizmo_request("PUT", f"/users/{user_id}/picture", {"Picture": picture_b64})
        except Exception as ex:
            log.exception("POST avatar failed for user %s", user_id)
            self._send_json(502, {"error": "upstream_unreachable", "detail": str(ex)})
            return

        if status not in (200, 204):
            self._send_json(502, {"error": "upstream_rejected", "status": status, "detail": data})
            return

        AVATAR_INDEX.set_has_avatar(user_id, True)
        self._send_json(200, {"status": "ok"})


def main():
    host, port = CONFIG["listen_host"], CONFIG["listen_port"]
    server = ThreadingHTTPServer((host, port), AvatarProxyHandler)
    log.info("Grafit avatar proxy v%s listening on %s:%s -> %s",
             PROXY_VERSION, host, port, CONFIG["gizmo_api_base"])
    try:
        server.serve_forever()
    except KeyboardInterrupt:
        log.info("Shutting down")
        server.shutdown()


if __name__ == "__main__":
    main()
