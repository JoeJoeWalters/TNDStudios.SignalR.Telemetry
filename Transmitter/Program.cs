using System;
using System.Collections.Generic;
using System.Threading;

namespace Transmitter
{
    class Program
    {
        private static List<TelemetryHandler> applications = new List<TelemetryHandler>();
        private static TelemetryHandler RandomApplication()
            =>applications[(Int32)((new Random()).NextDouble() * (Double)applications.Count)];

        static Program()
        {
            applications.Add(new TelemetryHandler("Mail Application", "https://localhost:44392/signalr/telemetry"));
            applications.Add(new TelemetryHandler("FTP Application", "https://localhost:44392/signalr/telemetry"));
            applications.Add(new TelemetryHandler("Invoicing Application", "https://localhost:44392/signalr/telemetry"));
            applications.Add(new TelemetryHandler("Payroll Application", "https://localhost:44392/signalr/telemetry"));
            applications.Add(new TelemetryHandler("Client 1 Application", "https://localhost:44392/signalr/telemetry"));
            applications.Add(new TelemetryHandler("Client 2 Application", "https://localhost:44392/signalr/telemetry"));
            applications.Add(new TelemetryHandler("Client 3 Application", "https://localhost:44392/signalr/telemetry"));
            applications.ForEach(app => app.Connect());
        }

        static void Main(string[] args)
        {
            while (true)
            {
                TelemetryHandler telemetryHandler = RandomApplication();

                Double randomVal = (new Random()).NextDouble();

                if (randomVal < 0.1)
                    telemetryHandler.Heartbeat();

                if (randomVal > 0.9)
                    telemetryHandler.Error("Shit went sideways!");

                telemetryHandler.Send("Record", (Int32)(randomVal * 100));
                Thread.Sleep((Int32)((new Random()).NextDouble() * 2000));
            }
        }
    }
}
