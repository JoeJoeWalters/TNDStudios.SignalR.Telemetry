"use strict";

var app = new Vue({
    el: '#contentcontainer',
    data: {
        page: new tndStudios.models.telemetry.page()
    },
    computed: {

    },
    watch: {

    },
    methods: {

        load: function () {

            var connection = new signalR.HubConnectionBuilder().withUrl("/signalr/telemetry").build();

            //Disable send button until connection is established
            document.getElementById("sendButton").disabled = true;

            connection.on("ReceiveMetric", function (applicationName, property, metric) {
                //var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
                debugger;
                var encodedMsg = applicationName + ": Processed " + metric + " " + property + "(s)";
                $.notify(encodedMsg, { autoHideDelay: 10000, className: "info" }); //https://notifyjs.jpillora.com/
            });

            connection.on("ReceiveHeartbeat", function (applicationName) {
                //var msg = applicationName.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
                var encodedMsg = applicationName + ": Heartbeat";
                $.notify(encodedMsg, { autoHideDelay: 30000, className: "success", globalPosition: "top left" }); //https://notifyjs.jpillora.com/
            });

            connection.on("ReceiveError", function (applicationName, errorMessage) {
                //var msg = errorMessage.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
                var encodedMsg = applicationName + ": Error '" + errorMessage + "'";
                $.notify(encodedMsg, { autoHideDelay: 30000, className: "error", globalPosition: "bottom left" }); //https://notifyjs.jpillora.com/
            });

            connection.start().then(function () {
                document.getElementById("sendButton").disabled = false;
            }).catch(function (err) {
                return console.error(err.toString());
            });

            document.getElementById("sendButton").addEventListener("click", function (event) {
                var user = document.getElementById("userInput").value;
                var message = document.getElementById("messageInput").value;
                connection.invoke("SendMessage", user, message).catch(function (err) {
                    return console.error(err.toString());
                });
                event.preventDefault();
            });

            /*
            app.page.currentToken = "No Token";
            app.page.currentSearch.clear();
            app.page.searchRunning = false;
            */
        }
    }
});

// On load ready
$(function ()
{
    app.load(); // Initialise the Vue code
});
