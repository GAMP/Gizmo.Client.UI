import axios from "axios";

window.ExternalFunctions = class ExternalFunctions {
  static async testAlert() {
    const response = await axios.get("https://catfact.ninja/fact");
    alert(response.data.fact);
  }
};
