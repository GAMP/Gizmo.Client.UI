window.ClientAPI = class ClientAPI {
  static DotnetObjectReference;

  static SetDotnetObjectReference(value) {
    ClientAPI.DotnetObjectReference = value;
  }

  static async SetUsernameAsync(username) {
    await this.DotnetObjectReference.invokeMethodAsync(
      "SetUsernameAsync",
      username
    );
  }
  static async SetPasswordAsync(username) {
    await this.DotnetObjectReference.invokeMethodAsync(
      "SetPasswordAsync",
      username
    );
  }
};
