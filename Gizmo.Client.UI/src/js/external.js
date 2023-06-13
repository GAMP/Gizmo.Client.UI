import axios from "axios";

window.ExternalFunctions = class ExternalFunctions {
  static async testAlert() {
    const response = await axios.get("https://catfact.ninja/fact");
    alert(response.data.fact);
  }

  static Advertisement = class Advertisement {
    /**
     * Event that is called when the advertisement is loaded
     * @param {string} title title of the ADS
     */
    static async OnLoad(title) {
      switch (title) {
        case "Custom HTML of an Advertisement":
          alert(`'${title}' was loaded.`);
          break;
        default:
          break;
      }
    }
  };
};
