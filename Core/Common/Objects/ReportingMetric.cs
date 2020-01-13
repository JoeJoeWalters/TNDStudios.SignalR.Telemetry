using Newtonsoft.Json;
using System;

namespace TNDStudios.SignalR.Telemetry.Objects
{
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
}
