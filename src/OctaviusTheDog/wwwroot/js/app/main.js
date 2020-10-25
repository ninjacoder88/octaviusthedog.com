define(function (require) {
    var $ = require("jquery");
    var ko = require("knockout");

    function ViewModel() {
        var self = this;
        self.pictures = ko.observableArray([]);
        self.go = function () {
            alert("went");
        };
    };

    ko.applyBindings(new ViewModel(), document.getElementById("container"));
});