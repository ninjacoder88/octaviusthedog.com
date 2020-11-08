define("octaviusModal",
    function () {
        "use strict";

        return {
            self: this,

            showModal: function (obj) {
                var titleElement = document.getElementById("octavius-modal-title");
                var messageElement = document.getElementById("octavius-modal-message");

                if (obj.title) {
                    titleElement.value = obj.title;
                }
                if (obj.message) {
                    messageElement.value = obj.message;
                }

                $("#octavius-modal").modal("show");
            }
        };
    });