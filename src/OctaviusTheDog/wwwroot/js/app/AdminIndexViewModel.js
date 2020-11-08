require(["jquery", "knockout"],
    function ($, ko) {
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
                }).done(function (response) {
                    alert("success");
                }).fail(function () {
                    alert("failure");
                });
            }
        }

        ko.applyBindings(new ViewModel());
    });