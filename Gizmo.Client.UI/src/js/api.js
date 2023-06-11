window.ClientAPI = class ClientAPI {
  static DotnetObjectReference;

  static SetDotnetObjectReference(value) {
    ClientAPI.DotnetObjectReference = value;
  }

  static async SetUsernameAsync(username) {
    await ClientAPI.DotnetObjectReference.invokeMethodAsync(
      "SetUsernameAsync",
      username
    );
  }
  static async SetPasswordAsync(username) {
    await ClientAPI.DotnetObjectReference.invokeMethodAsync(
      "SetPasswordAsync",
      username
    );
  }
};
