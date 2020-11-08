require(["jquery", "knockout", "octaviusModal", "bootstrap"],
    function ($, ko, modal) {
        "use strict";

        function Picture(obj) {
            var self = this;
            self.url = obj.url;
            self.name = obj.blobName;
        }

        function ViewModel() {
            var self = this;
            self.pictures = ko.observableArray([]);
            self.loading = ko.observable(false);

            self.initialize = function () {
                self.loading(true);
                self.pictures([]);

                $.ajax({
                    method: "GET",
                    url: "/home/GetPictures"
                }).done(function (data) {
                    if (data.success === true) {
                        data.pictures.forEach(picture => {
                            self.pictures.push(new Picture(picture));
                        });
                    } else {
                        modal.showModal(data.message);
                    }
                }).fail(function (a, b, c) {
                    modal.showModal("Something seems to have gone wrong while loading pictures");
                }).always(function () {
                    self.loading(false);
                });
            };

            self.initialize();
        };

        ko.applyBindings(new ViewModel(), document.getElementById("container"));
    });
   