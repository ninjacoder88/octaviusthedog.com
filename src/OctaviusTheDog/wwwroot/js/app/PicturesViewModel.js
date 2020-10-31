require(["jquery", "knockout"],
    function ($, ko) {
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
                }).done(function (picturesResponse) {
                    if (picturesResponse.success === true) {

                        picturesResponse.pictures.forEach(picture => {
                            self.pictures.push(new Picture(picture));
                        });

                    } else {
                        window.alert("Something went wrong while loading the pictures");
                    }
                }).fail(function (a, b, c) {
                    //console.error({ a: a, b: b, c: c });
                    window.alert("Something went wrong while loading the pictures");
                }).always(function () {
                    self.loading(false);
                });
            };

            self.initialize();
        };

        ko.applyBindings(new ViewModel(), document.getElementById("container"));
    });
   