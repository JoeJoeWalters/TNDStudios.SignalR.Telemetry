using Newtonsoft.Json;
using System;

namespace TNDStudios.SignalR.Telemetry.Objects
{
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
}
