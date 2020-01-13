using Newtonsoft.Json;
using System.Collections.Generic;

namespace TNDStudios.SignalR.Telemetry.Objects
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
}
