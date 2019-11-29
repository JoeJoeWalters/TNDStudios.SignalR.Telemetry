using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TNDStudios.SignalR.Telemetry.Hubs
{
    /// <summary>
    /// Summary of what has been happening since the last time the services was 
    /// restarted used for populating client warboards etc. when they first load before
    /// SignalR takes over transmission to the clients
    /// </summary>
    [JsonObject]
    public class ReportingSummary
    {
        [JsonProperty(PropertyName = "applications", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ReportingApplication> Applications { get; set; } = new List<ReportingApplication>() { };
    }

    /// <summary>
    /// Base class to define that all objects have an Id and a "received" date and time
    /// </summary>
    [JsonObject]
    public class ReportingObjectBase
    {
        [JsonProperty(PropertyName = "id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public String Id { get; set; }

        [JsonProperty(PropertyName = "receivedDateTime", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime ReceivedDateTime { get; set; }

        public ReportingObjectBase()
        {
            Id = Guid.NewGuid().ToString();
            ReceivedDateTime = DateTime.UtcNow;
        }
    }

    [JsonObject]
    public class ReportingMetric : ReportingObjectBase
    {
        [JsonProperty(PropertyName = "property", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public String Property { get; set; }

        [JsonProperty(PropertyName = "value", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public String Value { get; set; }

        /// <summary>
        /// Default constructor using base to set up core values
        /// </summary>
        public ReportingMetric() : base() { }
    }

    [JsonObject]
    public class ReportingError : ReportingObjectBase
    {
        [JsonProperty(PropertyName = "message", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public String Message { get; set; }

        /// <summary>
        /// Default constructor using base to set up core values
        /// </summary>
        public ReportingError() : base() { }
    }

    [JsonObject]
    public class ReportingHeartbeat : ReportingObjectBase
    {
        /// <summary>
        /// Default constructor using base to set up core values
        /// </summary>
        public ReportingHeartbeat() : base() { }
    }

    [JsonObject]
    public class ReportingApplication : ReportingObjectBase
    {
        [JsonProperty(PropertyName = "name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public String Name { get; set; }

        [JsonProperty(PropertyName = "metrics", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ReportingMetric> Metrics { get; set; }

        [JsonProperty(PropertyName = "errors", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ReportingError> Errors { get; set; }

        [JsonProperty(PropertyName = "heartbeats", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ReportingHeartbeat> Heartbeats { get; set; }

        /// <summary>
        /// Default constructor using base to set up core values
        /// </summary>
        public ReportingApplication() : base()
        {
            Name = String.Empty;
            Metrics = new List<ReportingMetric>();
            Errors = new List<ReportingError>();
            Heartbeats = new List<ReportingHeartbeat>();
        }
    }

    public class TelemetryHub : Hub
    {
        /// <summary>
        /// Dictionary of applications that have been reported against by the clients
        /// </summary>
        private static Dictionary<String, ReportingApplication> applications;

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
                lock (applications)
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

        public async Task SendHeartbeat(string applicationName)
        {
            ReportingApplication application = GetApplication(applicationName);
            application.Heartbeats.Add(new ReportingHeartbeat() { });
            await Clients.All.SendAsync("ReceiveHeartbeat", applicationName);
        }

        public async Task SendError(string applicationName, string errorMessage)
        {
            ReportingApplication application = GetApplication(applicationName);
            application.Errors.Add(new ReportingError() { Message = errorMessage });
            await Clients.All.SendAsync("ReceiveError", applicationName, errorMessage);
        }
    }
}
