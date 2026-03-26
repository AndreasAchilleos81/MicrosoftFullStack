window.skillSnap = {
    onExit: function (callback) {
        window.addEventListener("beforeunload", () => {
            callback.invokeMethodAsync("ClearLocalStorage");
        });
    }
};