require(["jquery", "knockout", "octaviusModal", "bootstrap"],
    function ($, ko, modal) {
        "use strict";

        function Picture(obj) {
            var self = this;
            self.previewUrl = obj.previewUrl;
            self.originalUrl = obj.originalUrl;
            self.title = obj.title;

            self.show = function () {
                var imageElement = document.getElementById("picture-modal-image");
                imageElement.setAttribute("src", "");

                var titleElement = document.getElementById("picture-modal-title");
                titleElement.innerText = "";

                imageElement.setAttribute("src", self.originalUrl);
                titleElement.innerText = self.title;

                $("#picture-modal").modal("show");
            };
        }

        function ViewModel() {
            var self = this;
            self.pictures = ko.observableArray([]);
            self.loading = ko.observable(false);
            self.emailAddress = ko.observable("");
            var pageNumber = 1;

            function getPictures(pageNumber, successFunc) {
                self.loading(true);
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
                    modal.showModal({ title: "Error", message: "Something seems to have gone wrong while loading pictures"});
                }).always(function () {
                    self.loading(false);
                });
            };

            self.subscribe = function() {
                if (self.emailAddress() === "") {
                    return;
                }

                if (self.emailAddress().indexOf("@") === -1) {
                    return;
                }

                $.ajax({
                    method: "POST",
                    url: "/home/Subscribe",
                    data: JSON.stringify({ emailAddress: self.emailAddress() }),
                    dataType: "json",
                    contentType: "application/json"
                }).done(function () {
                    self.emailAddress("");
                    modal.showModal({ title: "Success", message: "Thank you for subscribing"});
                }).fail(function (a, b, c) {
                    modal.showModal({ title: "Error", message: "Something seems to have gone wrong while subscribing" });
                })
            };

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