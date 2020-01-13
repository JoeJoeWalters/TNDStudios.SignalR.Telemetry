using Newtonsoft.Json;
using System;

namespace TNDStudios.SignalR.Telemetry.Objects
{
    [JsonObject]
    public class ReportingHeartbeat : ReportingObjectBase
    {
        /// <summary>
        /// Default constructor using base to set up core values
        /// </summary>
        public ReportingHeartbeat() : base() { }

        [JsonProperty(PropertyName = "nextRunTime", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime NextRunTime { get; set; }
    }
}
