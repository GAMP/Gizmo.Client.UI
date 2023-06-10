window.ClientAPI = class ClientAPI {
  static dotnetObjectReference;

  static SetDotnetObjectReference(value) {
    ClientAPI.dotnetObjectReference = value;
  }

  static async SetUsernameAsync(username) {
    await ClientAPI.dotnetObjectReference.invokeMethodAsync(
      "SetUsernameAsync",
      username
    );
  }
  static async SetPasswordAsync(username) {
    await ClientAPI.dotnetObjectReference.invokeMethodAsync(
      "SetPasswordAsync",
      username
    );
  }
};
