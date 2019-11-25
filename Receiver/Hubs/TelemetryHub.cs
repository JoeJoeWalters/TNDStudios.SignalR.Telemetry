using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TNDStudios.SignalR.Telemetry.Hubs
{
    public class TelemetryHub : Hub
    {
        public async Task SendMetric(string applicationName, string property, string metric)
        {
            await Clients.All.SendAsync("ReceiveMetric", applicationName, property, metric);
        }

        public async Task SendHeartbeat(string applicationName)
        {
            await Clients.All.SendAsync("ReceiveHeartbeat", applicationName);
        }

        public async Task SendError(string applicationName, string errorMessage)
        {
            await Clients.All.SendAsync("ReceiveError", applicationName, errorMessage);
        }
    }
}
