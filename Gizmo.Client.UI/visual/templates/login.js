// The idle sign-in screen without a club wallpaper: the shell's own atmosphere, the
// moving gradient on it and the host number. Mirrors the background layers of
// Shared/_Layout_Login.razor; the sign-in card itself is the vendor's form and is not
// reproduced. The harness parks infinite animations at frame zero, so this shows the
// gradient at its starting position only.
"use strict";

const { esc } = require("../lib/html");

// d: { pc, card }
function render(d) {
  return `<div class="giz-main-container">
  <div class="giz-login-content giz-login-content--own-bg${d.card ? " collapsed" : ""}">
    <div class="giz-login-hardcoded-bg">
      <div class="gg-flow" aria-hidden="true">
        <span class="gg-flow__sheet gg-flow__sheet--1"></span>
        <span class="gg-flow__sheet gg-flow__sheet--2"></span>
      </div>
    </div>
    <div class="giz-login-hardcoded-bg__icons">
      <i class="ph-fill ph-game-controller"></i>
      <i class="ph-fill ph-desktop-tower"></i>
      <i class="ph-fill ph-headset"></i>
    </div>
    <div class="giz-host-number top-right">PC ${esc(d.pc || 12)}</div>
    <div class="giz-login-warnings"></div>
  </div>
</div>`;
}

module.exports = { render };
