window.ClientAPI = class ClientAPI {
  static dotnetObjectReference;

  static SetDotnetObjectReference(value) {
    this.dotnetObjectReference = value;
  }

  static async SetUsernameAsync(username) {
    await this.dotnetObjectReference.invokeMethodAsync(
      "SetUsernameAsync",
      username
    );
  }
  static async SetPasswordAsync(username) {
    await this.dotnetObjectReference.invokeMethodAsync(
      "SetPasswordAsync",
      username
    );
  }
};
