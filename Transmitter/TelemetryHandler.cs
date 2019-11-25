using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Transmitter
{
    public class TelemetryHandler
    {
        private HubConnection connection;
        private String applicationName;

        public TelemetryHandler(String applicationName, String uri)
        {
            this.applicationName = applicationName;
            connection = new HubConnectionBuilder()
                .WithUrl(new Uri(uri))
                .WithAutomaticReconnect(new TelemetryRetryPolicy())
                .Build();
        }

        public Boolean Connect()
        {
            Int32 attempts = 5;
            CancellationToken token = new CancellationToken();

            // Keep trying to until we can start or the token is canceled.
            while (true)
            {
                try
                {
                    connection.StartAsync(token).Wait();
                    return true;
                }
                catch when (token.IsCancellationRequested)
                {
                    return false;
                }
                catch
                {
                    // Failed to connect, trying again in 5000 ms.
                    attempts--;
                    if (0 == attempts) return false;
                    Task.Delay(5000);
                }
            }
        }

        public Boolean Heartbeat(String application)
        {
            if (connection.State == HubConnectionState.Connected)
            {
                try
                {
                    connection.InvokeAsync("SendHeartbeat", applicationName).Wait();
                    Console.WriteLine("Sent Heartbeat");
                }
                catch
                {
                    return false;
                }
            }
            else
                return false;

            return true;
        }

        public Boolean Error(String error)
        {
            if (connection.State == HubConnectionState.Connected)
            {
                try
                {
                    connection.InvokeAsync("SendError", applicationName, error).Wait();
                    Console.WriteLine("Sent Error");
                }
                catch
                {
                    return false;
                }
            }
            else
                return false;

            return true;
        }

        public Boolean Send(String property, Int32 metric)
        {
            if (connection.State == HubConnectionState.Connected)
            {
                try
                {
                    connection.InvokeAsync("SendMetric", applicationName, property, metric.ToString()).Wait();
                    Console.WriteLine("Sent Metric");
                }
                catch
                {
                    return false;
                }
            }
            else
                return false;

            return true;
        }
    }

    public class TelemetryRetryPolicy : IRetryPolicy
    {
        private readonly Random _random = new Random();

        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            // If we've been reconnecting for less than 60 seconds so far,
            // wait between 0 and 10 seconds before the next reconnect attempt.
            if (retryContext.ElapsedTime < TimeSpan.FromSeconds(60))
            {
                return TimeSpan.FromSeconds(_random.Next() * 10);
            }
            else
            {
                // If we've been reconnecting for more than 60 seconds so far, stop reconnecting.
                return null;
            }
        }
    }

}
