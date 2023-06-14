import axios from "axios";

window.ExternalFunctions = class ExternalFunctions {
  static async testAlert() {
    const response = await axios.get("https://catfact.ninja/fact");
    alert(response.data.fact);
  }

  static Advertisement = class Advertisement {
    /**
     * Event that is called when the advertisement is loaded
     * @param {Map} parameters is a map of parameters that are passed to the application
     * parameters.Data is a string parameter that contains the data that is passed from the this function of the custom HTML
     * parameters.Key is a key name of the parameter value of the Client.UI application
     */
    static async OnLoad(parameters) {
      const message = !parameters
        ? "Advertisement is loaded without parameters."
        : `Advertisement is loaded with parameters:
            \n0. All the parameters: '${JSON.stringify(parameters)}'
            \n1. Client.UI parameter: '${parameters.Title}'
            \n2. Custom parameters of the function: ${parameters.Data}`;

      console.log(message);
    }
  };
};
