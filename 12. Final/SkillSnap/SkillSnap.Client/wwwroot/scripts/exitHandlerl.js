window.skillSnap = {
    onExit: function (callback) {
        if (!window._skillSnapExitRegistered) {
            window.addEventListener("beforeunload", () => {
                callback.invokeMethodAsync("ClearLocalStorage");
            });
            window._skillSnapExitRegistered = true;
        }
    }
};
