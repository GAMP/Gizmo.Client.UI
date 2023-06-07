class ExternalFunctions {
  static testFunction() {
    alert("Test external JavaScript alert!");
  }
  static loadScript(externalScript) {
    var script = document.createElement("script");
    script.textContent = externalScript;
    script.type = "text/javascript";
    document.getElementsByTagName("body")[0].appendChild(script);
  }
}
