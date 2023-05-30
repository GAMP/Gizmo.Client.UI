class ClientFunctions {
  static dotnetObjectReference;
  static SetDotnetObjectReference(value) {
    ClientFunctions.dotnetObjectReference = value;
  }

  static async SetUsernameAsync(username) {
    await ClientFunctions.dotnetObjectReference.invokeMethodAsync(
      "SetUsernameAsync",
      username
    );
  }
  static async SetPasswordAsync(username) {
    await ClientFunctions.dotnetObjectReference.invokeMethodAsync(
      "SetPasswordAsync",
      username
    );
  }

  /**
   * Subscribes to browser full screen change event.
   * @param {string} callBackName callBack function name.
   */
  static async SubscribeOnFullScreenChange(callBackName) {
    try {
      subscribeFullScreenChanged(callBackName);
    } catch (error) {
      ClientFunctions.dotnetObjectReference.invokeMethodAsync(
        callBackName,
        false,
        error.message
      );
    }
  }
  /**
   * Unsubscribes from browser full screen change event.
   * @param {string} callBackName callBack function name.
   */
  static async UnsubscribeOnFullScreenChange(callBackName) {
    try {
      unsubscribeFullScreenChanged(callBackName);
    } catch (error) {
      ClientFunctions.dotnetObjectReference.invokeMethodAsync(
        callBackName,
        false,
        error.message
      );
    }
  }
}

function isFullScreenChangeHandler(callBackName) {
  return function (event) {
    if (
      document.fullScreen ||
      document.mozFullScreen ||
      document.webkitIsFullScreen ||
      document.msFullscreenElement
    ) {
      ClientFunctions.dotnetObjectReference.invokeMethodAsync(
        callBackName,
        true,
        null
      );
    } else {
      ClientFunctions.dotnetObjectReference.invokeMethodAsync(
        callBackName,
        false,
        null
      );
    }
  };
}
function subscribeFullScreenChanged(callBackName) {
  const handler = isFullScreenChangeHandler(callBackName);

  document.addEventListener("fullscreenchange", handler, false);
  document.addEventListener("webkitfullscreenchange", handler, false);
  document.addEventListener("mozfullscreenchange", handler, false);
}
function unsubscribeFullScreenChanged(callBackName) {
  const handler = isFullScreenChangeHandler(callBackName);

  document.removeEventListener("fullscreenchange", handler, false);
  document.removeEventListener("webkitfullscreenchange", handler, false);
  document.removeEventListener("mozfullscreenchange", handler, false);
}
