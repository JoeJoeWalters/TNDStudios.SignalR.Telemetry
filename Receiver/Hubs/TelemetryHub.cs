using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TNDStudios.SignalR.Telemetry.Objects;

namespace TNDStudios.SignalR.Telemetry.Hubs
{
    public class TelemetryHub : Hub
    {
        /// <summary>
        /// Dictionary of applications that have been reported against by the clients
        /// </summary>
        private static Dictionary<String, ReportingApplication> applications;

        // Locking object for the dictionary rather than putting a lock on the dictionary itself
        // so we can dirty read the metrics without slowing down the app but creates and deletes
        // can be locked, we also don't want to lock array pushes to sub-items
        private static Object lockingObject = new Object();

        /// <summary>
        /// On absolute start then set up the arrays needed across sessions
        /// </summary>
        static TelemetryHub()
        {
            applications = new Dictionary<String, ReportingApplication>();
        }

        /// <summary>
        /// Check to see if the application has been received before, if not create it
        /// and return the value that was either found or created
        /// </summary>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        private ReportingApplication GetApplication(String applicationName)
        {
            // Format the application name so it's consistent
            String searchString = (applicationName ?? String.Empty).ToLower().Trim();

            // See if the application has already been received
            if (!applications.ContainsKey(searchString))
            {
                // Lock the applications object whilst we are inserting
                // as it could be getting another request at the same time
                // and we don't want to overwrite it
                lock (lockingObject)
                {
                    // Now the application is locked, check again just incase someone 
                    // else wrote whilst we were waiting to lock
                    if (applications.ContainsKey(searchString))
                        return applications[searchString];

                    // After the second check the application still wasn't there
                    // Create the new application for reporting and adding to the cache
                    ReportingApplication newApplication = new ReportingApplication() { Name = applicationName.Trim() };

                    // Cache it for someone else
                    applications[searchString] = newApplication;

                    // Return the application
                    return newApplication;
                } // Unlock so others can create applications
            }
            else
                return applications[searchString]; // Send the existing one back
        }

        /// <summary>
        /// Generate a reporting summary for warboards etc.
        /// </summary>
        /// <returns>The consolidated reporting summary</returns>
        public ReportingSummary GetReportingSummary()
        {
            // Create a blank report as we won't be returning everything
            ReportingSummary reportingSummary =
                new ReportingSummary()
                {
                    Applications = new List<ReportingApplication>()
                };

            // Loop all the applications

            return reportingSummary;
        }

        public async Task SendMetric(string applicationName, string property, string metric)
        {
            ReportingApplication application = GetApplication(applicationName);
            application.Metrics.Add(new ReportingMetric() { Property = property, Value = metric });
            await Clients.All.SendAsync("ReceiveMetric", applicationName, property, metric);
        }

        public async Task SendHeartbeat(string applicationName, DateTime nextRunTime)
        {
            ReportingApplication application = GetApplication(applicationName);
            application.NextRunTime = nextRunTime;
            application.Heartbeats.Add(new ReportingHeartbeat() { NextRunTime = nextRunTime });
            await Clients.All.SendAsync("ReceiveHeartbeat", applicationName, nextRunTime);
        }

        public async Task SendError(string applicationName, string errorMessage)
        {
            ReportingApplication application = GetApplication(applicationName);
            application.Errors.Add(new ReportingError() { Message = errorMessage });
            await Clients.All.SendAsync("ReceiveError", applicationName, errorMessage);
        }
    }
}
