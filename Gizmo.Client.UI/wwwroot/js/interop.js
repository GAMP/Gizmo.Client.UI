class ClientFunctions {
    static dotnetObjectReference;
    static SetDotnetObjectReference(value) {
        ClientFunctions.dotnetObjectReference = value;
    }

    static async SetUsernameAsync(username) { await ClientFunctions.dotnetObjectReference.invokeMethodAsync('SetUsernameAsync', username); }
    static async SetPasswordAsync(username) { await ClientFunctions.dotnetObjectReference.invokeMethodAsync('SetPasswordAsync', username); }

    /**
     * Sets image source on specified image element.
     * @param {HTMLElement} imageElement HTML Image element.
     * @param {any} imageStream Image stream.
     */
    static async SetImageSourceAsync(imageElement, imageStream) {

        if (imageElement === null) //no idea why but in some cases ref is equal to null
            return;

        const arrayBuffer = await imageStream.arrayBuffer();
        const blob = new Blob([arrayBuffer]);
        const url = URL.createObjectURL(blob);
        imageElement.onload = () => {
            URL.revokeObjectURL(url);
        }
        imageElement.src = url;
    }
}