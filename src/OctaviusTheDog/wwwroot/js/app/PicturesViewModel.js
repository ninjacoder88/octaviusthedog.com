require(["jquery", "knockout"],
    function ($, ko) {
        "use strict";

        function Picture(obj) {
            var self = this;
            self.baseUrl = obj.baseUrl;
            self.name = obj.name;
            self.url = self.baseUrl + self.name;
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
                }).done(function (response) {
                    if (response.success === true) {
                        //console.log(response.names);
                        response.names.forEach(name => {
                            self.pictures.push(new Picture({ baseUrl: response.baseUrl, name: name }))
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
   