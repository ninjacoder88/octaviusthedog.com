requirejs.config({
    baseUrl: "js/lib",
    paths: {
        app: "../app",
        knockout: "https://cdnjs.cloudflare.com/ajax/libs/knockout/3.5.0/knockout-min",
        jquery: "https://code.jquery.com/jquery-3.5.1.min",
        bootstrap: "https://cdn.jsdelivr.net/npm/bootstrap@4.5.3/dist/js/bootstrap.bundle.min.js"
    },
    onNodeCreated: function (node, config, module, path) {
        var sri = {
            bootstrap: "sha384-ho+j7jyWK8fNQe+A12Hb8AhRq26LrZ/JpcUGGOn+Y7RsweNrtN/tE3MoK7ZeZDyx"
        };

        if (sri[module]) {
            node.setAttribute("integrity", sri[module]);
            node.setAttribute("crossorigin", "anonymous")
        }
    }
});

requirejs(["app/main"]);