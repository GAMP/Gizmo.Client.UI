class ClientFunctions {
    static dotnetObjectReference;
    static SetDotnetObjectReference(value) {
        ClientFunctions.dotnetObjectReference = value;
    }

    static async SetUsernameAsync(username) { await ClientFunctions.dotnetObjectReference.invokeMethodAsync('SetUsernameAsync', username); }
    static async SetPasswordAsync(username) { await ClientFunctions.dotnetObjectReference.invokeMethodAsync('SetPasswordAsync', username); }
}