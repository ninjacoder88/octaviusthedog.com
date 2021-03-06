﻿require(["jquery", "knockout", "octaviusModal", "bootstrap"],
    function ($, ko, modal) {
        "use strict";

        function ViewModel() {
            var self = this;
            self.title = ko.observable("");

            self.upload = function () {
                var element = document.getElementById("fileUpload");
                var files = element.files;
                var formData = new FormData();

                for (var f = 0; f < files.length; f++) {
                    formData.append("files", files[f]);
                }
                formData.append("title", self.title());

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
                        self.title("");
                    } else {
                        modal.showModal({ title: "Error", message: data.message });
                    }
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    console.error({ jqXHR: jqXHR, textStatus: textStatus, errorThrown: errorThrown });
                    alert("failure");
                });
            };

            self.sendUpdate = function () {
                $.ajax({
                    url: "/admin/NotifyUpdate",
                    method: "GET",
                }).done(function (data) {
                    if (data.success === true) {

                    } else {
                        modal.showModal({ title: "Error", message: data.message });
                    }
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    console.error({ jqXHR: jqXHR, textStatus: textStatus, errorThrown: errorThrown });
                    alert("failure");
                });
            };
        }

        ko.applyBindings(new ViewModel());
    });