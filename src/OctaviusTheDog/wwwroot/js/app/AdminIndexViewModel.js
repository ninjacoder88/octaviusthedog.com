require(["jquery", "knockout", "octaviusModal", "bootstrap"],
    function ($, ko, modal) {
        "use strict";

        function ViewModel() {
            var self = this;

            self.upload = function () {
                var element = document.getElementById("fileUpload");
                var files = element.files;
                var formData = new FormData();

                for (var f = 0; f < files.length; f++) {
                    formData.append("files", files[f]);
                }

                $.ajax({
                    url: "/admin/upload",
                    method: "POST",
                    data: formData,
                    processData: false,
                    contentType: false,
                }).done(function (data, textStatus, jqXHR) {
                    if (data.success === true) {
                        //remove files
                        element.value = null;
                    } else {
                        modal.showModal({ title: "Error", message: data.message });
                    }
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    alert("failure");
                });
            }
        }

        ko.applyBindings(new ViewModel());
    });