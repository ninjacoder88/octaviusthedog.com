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
            var pageNumber = 1;

            function getPictures(pageNumber, successFunc) {
                self.loading = true;
                self.pictures([]);

                $.ajax({
                    method: "GET",
                    url: "/home/GetPictures?pageNumber=" + pageNumber
                }).done(function (data) {
                    if (data.success === true) {
                        data.pictures.forEach(picture => {
                            self.pictures.push(new Picture(picture));
                        });
                        successFunc();
                    } else {
                        modal.showModal(data.message);
                    }
                }).fail(function (a, b, c) {
                    modal.showModal("Something seems to have gone wrong while loading pictures");
                }).always(function () {
                    self.loading(false);
                });
            }

            self.next = function () {
                var nextPage = pageNumber + 1;
                getPictures(nextPage,
                    function () {
                        pageNumber = nextPage;
                    });
            };

            self.previous = function () {
                var previousPage = pageNumber - 1;
                getPictures(previousPage,
                    function () {
                        pageNumber = previousPage;
                    });
            };

            self.initialize = function () {
                getPictures(pageNumber, function () {
                });
            };

            self.initialize();
        };

        ko.applyBindings(new ViewModel(), document.getElementById("container"));

    });