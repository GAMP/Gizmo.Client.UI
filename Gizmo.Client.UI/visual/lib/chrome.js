// Headless Chrome as the renderer, driven over the DevTools protocol. Nothing is
// installed for this: every club machine and every developer machine already has Chrome
// or Edge, and Node 22 speaks WebSocket on its own.
//
// Why the protocol rather than chrome --screenshot: the one-shot mode captures whenever
// its virtual time runs out, which with several Chromes competing for the CPU was
// sometimes before the fonts and images had been painted. Here the page itself says
// when it is ready - fonts loaded, every image decoded, animations finished - and only
// then is the picture taken. Two runs draw the same pixels.
"use strict";

const fs = require("fs");
const os = require("os");
const path = require("path");
const { spawn, execFile } = require("child_process");

const CANDIDATES = [
  process.env.GRAFIT_CHROME,
  process.env.CHROME_PATH,
  "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe",
  "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe",
  path.join(process.env.LOCALAPPDATA || "", "Google", "Chrome", "Application", "chrome.exe"),
  "C:\\Program Files\\Microsoft\\Edge\\Application\\msedge.exe",
  "C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe",
  "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome",
  "/usr/bin/google-chrome",
  "/usr/bin/chromium",
  "/usr/bin/chromium-browser",
];

function find() {
  for (const candidate of CANDIDATES) {
    if (candidate && fs.existsSync(candidate)) return candidate;
  }
  throw new Error("No Chrome or Edge found. Install one, or point GRAFIT_CHROME at the executable.");
}

// For the report header. On Windows the executable prints nothing for --version; the
// installer keeps the build in a directory named after it, next to the executable.
function version(chrome) {
  const name = path.basename(chrome, path.extname(chrome));
  if (process.platform === "win32") {
    try {
      const dirs = fs.readdirSync(path.dirname(chrome)).filter((d) => /^\d+(\.\d+){3}$/.test(d));
      if (dirs.length) return Promise.resolve(`${name} ${dirs.sort().pop()}`);
    } catch (_) { /* fall through */ }
    return Promise.resolve(name);
  }
  return new Promise((resolve) => {
    execFile(chrome, ["--version"], { timeout: 15000, windowsHide: true }, (error, stdout) => {
      resolve(error ? name : String(stdout).trim());
    });
  });
}

// Runs in the page before every capture. Finite animations are pushed to their last
// frame, so an entrance is over and a progress bar is full; infinite ones - a breathing
// icon, a sheen - are parked at their first frame, or each run would sample them at a
// different moment. The capture itself forces a fresh frame.
const READY = `(async () => {
  await document.fonts.ready;
  await Promise.all(Array.from(document.images).map((img) => img.decode().catch(() => {})));
  for (const animation of document.getAnimations()) {
    try {
      const timing = animation.effect && animation.effect.getTiming();
      if (timing && timing.iterations === Infinity) {
        animation.pause();
        animation.currentTime = 0;
      } else {
        animation.finish();
      }
    } catch (_) {}
  }
  return true;
})()`;

class Browser {
  constructor(executable, lane) {
    this.executable = executable;
    this.lane = lane || 0;
    this.process = null;
    this.socket = null;
    this.nextId = 1;
    this.pending = new Map();
    this.listeners = new Map();
  }

  async launch() {
    if (typeof WebSocket !== "function")
      throw new Error("Node 22 or newer is needed for the visual tests (WebSocket built in).");

    const profile = path.join(os.tmpdir(), "grafit-visual", "profile-" + process.pid + "-" + this.lane);
    fs.rmSync(profile, { recursive: true, force: true });
    fs.mkdirSync(profile, { recursive: true });

    const args = [
      "--headless=new",
      "--remote-debugging-port=0",
      "--user-data-dir=" + profile,
      "--no-first-run",
      "--no-default-browser-check",
      "--disable-extensions",
      "--disable-sync",
      "--disable-gpu",
      "--mute-audio",
      "--allow-file-access-from-files",
      "--force-device-scale-factor=1",
      // Text is drawn the same way on every machine: no subpixel colour fringes, no
      // hinting that depends on the installed font engine.
      "--disable-lcd-text",
      "--font-render-hinting=none",
      // Scrollbars stay: one appearing where none was is exactly the kind of change
      // this exists to catch.
      "about:blank",
    ];

    this.process = spawn(this.executable, args, { windowsHide: true, stdio: ["ignore", "ignore", "pipe"] });
    this.profile = profile;

    const endpoint = await new Promise((resolve, reject) => {
      let text = "";
      const timer = setTimeout(() => reject(new Error("Chrome did not start within 30s:\n" + text)), 30000);
      this.process.stderr.on("data", (chunk) => {
        text += chunk;
        const match = text.match(/DevTools listening on (ws:\/\/\S+)/);
        if (match) { clearTimeout(timer); resolve(match[1]); }
      });
      this.process.on("exit", (code) => { clearTimeout(timer); reject(new Error("Chrome exited with code " + code + ":\n" + text)); });
    });

    this.socket = new WebSocket(endpoint);
    await new Promise((resolve, reject) => {
      this.socket.onopen = resolve;
      this.socket.onerror = () => reject(new Error("Could not connect to " + endpoint));
    });
    this.socket.onmessage = (event) => this.receive(JSON.parse(event.data));
  }

  receive(message) {
    if (message.id && this.pending.has(message.id)) {
      const { resolve, reject } = this.pending.get(message.id);
      this.pending.delete(message.id);
      if (message.error) reject(new Error(message.error.message));
      else resolve(message.result);
      return;
    }
    if (message.method) {
      const key = (message.sessionId || "") + ":" + message.method;
      const waiting = this.listeners.get(key);
      if (waiting) { this.listeners.delete(key); waiting(message.params); }
    }
  }

  send(method, params, sessionId) {
    const id = this.nextId++;
    return new Promise((resolve, reject) => {
      this.pending.set(id, { resolve, reject });
      this.socket.send(JSON.stringify({ id, method, params: params || {}, sessionId }));
    });
  }

  once(method, sessionId) {
    return new Promise((resolve) => this.listeners.set((sessionId || "") + ":" + method, resolve));
  }

  // The tab the shots are taken in. One per browser: a second tab would sit in the
  // background, where Chrome neither decodes images nor draws frames.
  async tab() {
    const { targetId } = await this.send("Target.createTarget", { url: "about:blank" });
    const { sessionId } = await this.send("Target.attachToTarget", { targetId, flatten: true });
    await this.send("Page.enable", {}, sessionId);
    await this.send("Runtime.enable", {}, sessionId);
    return new Tab(this, sessionId);
  }

  async close() {
    try { if (this.socket) this.socket.close(); } catch (_) {}
    if (this.process) {
      this.process.kill();
      await new Promise((resolve) => { this.process.on("exit", resolve); setTimeout(resolve, 3000); });
    }
    // Chrome's helper processes can hold the profile for a moment after the kill;
    // a directory left in the temp folder is not worth failing the run over.
    try { if (this.profile) fs.rmSync(this.profile, { recursive: true, force: true }); } catch (_) {}
  }
}

class Tab {
  constructor(browser, sessionId) {
    this.browser = browser;
    this.sessionId = sessionId;
  }

  // `scale` is the device pixel ratio (the promo shots take 2: a 1920x1080 page as a
  // 3840x2160 picture); `hover` is a selector whose first match is drawn in its :hover
  // state, the way the DevTools "force element state" does it.
  async screenshot(pageUrl, outPng, width, height, { scale = 1, hover = null } = {}) {
    const b = this.browser;
    await b.send("Emulation.setDeviceMetricsOverride", { width, height, deviceScaleFactor: scale, mobile: false }, this.sessionId);
    const loaded = b.once("Page.loadEventFired", this.sessionId);
    await b.send("Page.navigate", { url: pageUrl }, this.sessionId);
    await Promise.race([loaded, new Promise((_, reject) => setTimeout(() => reject(new Error("Timed out loading " + pageUrl)), 60000))]);
    await b.send("Runtime.evaluate", { expression: READY, awaitPromise: true }, this.sessionId);
    if (hover) {
      await b.send("DOM.enable", {}, this.sessionId);
      await b.send("CSS.enable", {}, this.sessionId);
      const { root } = await b.send("DOM.getDocument", { depth: 0 }, this.sessionId);
      const { nodeId } = await b.send("DOM.querySelector", { nodeId: root.nodeId, selector: hover }, this.sessionId);
      if (!nodeId) throw new Error("Nothing matches the hover selector " + hover);
      await b.send("CSS.forcePseudoState", { nodeId, forcedPseudoClasses: ["hover"] }, this.sessionId);
      // The forced state takes a frame to reach the screen.
      await b.send("Runtime.evaluate", { expression: "new Promise((r) => requestAnimationFrame(() => requestAnimationFrame(r)))", awaitPromise: true }, this.sessionId);
    }
    const { data } = await b.send("Page.captureScreenshot", { format: "png", captureBeyondViewport: false }, this.sessionId);
    fs.writeFileSync(outPng, Buffer.from(data, "base64"));
    return outPng;
  }
}

module.exports = { find, version, Browser };
