"use strict";

var app = new Vue({
    el: '#contentcontainer',
    data: {
        page: new tndStudios.models.telemetry.page()
    },
    computed: {

        // Sorted array so errors appear at the top
        sortedApplicationArray: function () {
            return this.page.applications.applicationArray.sort((a, b) =>
            {
                if (a.errors.length < b.errors.length)
                    return 1;
                else if (a.errors.length > b.errors.length)
                    return -1
                else
                    return 0;
            });
        }

    },
    watch: {

    },
    methods: {

        load: function () {

            var connection = new signalR.HubConnectionBuilder().withUrl("/signalr/telemetry").build();

            connection.on("ReceiveMetric", function (applicationName, property, metric) {
                var application = app.page.applications.addApplication(applicationName);
                var encodedMsg = "Processed " + metric + " " + property + "(s)";
                application.addMetric(encodedMsg);
                //$.notify(applicationName + " : " + encodedMsg, { autoHideDelay: 10000, className: "info", globalPosition: "top right" }); //https://notifyjs.jpillora.com/
            });

            connection.on("ReceiveHeartbeat", function (applicationName) {
                var application = app.page.applications.addApplication(applicationName);
                var encodedMsg = "Heartbeat";
                //$.notify(encodedMsg, { autoHideDelay: 5000, className: "success", globalPosition: "top right" }); //https://notifyjs.jpillora.com/
            });

            connection.on("ReceiveError", function (applicationName, errorMessage) {
                var application = app.page.applications.addApplication(applicationName);
                var encodedMsg = applicationName + ": Error '" + errorMessage + "'";
                application.addError(errorMessage);
                //$.notify(encodedMsg, { autoHideDelay: 30000, className: "error", globalPosition: "top right" }); //https://notifyjs.jpillora.com/
            });

            connection.start().then(function () {
                // Connection worked do something
            }).catch(function (err) {
                return console.error(err.toString());
            });

            // Render the charts
            var ctx = document.getElementById('chartDisplay1').getContext('2d');
            var chart = new Chart(ctx, {
                // The type of chart we want to create
                type: 'line',

                // The data for our dataset
                data: {
                    labels: ['180', '170', '160', '150', '140', '130', '120', '110', '100', '90', '80', '70', '60', '50', '40', '30', '20', '10'],
                    datasets: [{
                        label: 'Error Rate',
                        backgroundColor: 'rgb(255, 99, 132)',
                        borderColor: 'rgb(200, 69, 100)',
                        data: [0, 10, 5, 0, 20, 0, 0, 0, 0, 0, 0, 0, 5, 12, 20, 32, 45, 12]
                    }]
                },

                // Configuration options go here
                options: {}
            });

            var ctx2 = document.getElementById('chartDisplay2').getContext('2d');
            var chart2 = new Chart(ctx2, {
                // The type of chart we want to create
                type: 'line',

                // The data for our dataset
                data: {
                    labels: ['180', '170', '160', '150', '140', '130', '120', '110', '100', '90', '80', '70', '60', '50', '40', '30', '20', '10'],
                    datasets: [{
                        label: 'Metrics',
                        backgroundColor: 'rgb(132, 132, 255)',
                        borderColor: 'rgb(99, 99, 200)',
                        data: [0, 10, 5, 0, 20, 0, 0, 0, 0, 0, 0, 0, 5, 12, 20, 32, 45, 12]
                    }]
                },

                // Configuration options go here
                options: { }
            });

            /*
            connection.invoke("SendMessage", user, message).catch(function (err) {
                return console.error(err.toString());
            });
            */
        }
    }
});

// On load ready
$(function ()
{
    app.load(); // Initialise the Vue code
});
