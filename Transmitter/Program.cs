using System;
using System.Threading;

namespace Transmitter
{
    class Program
    {
        static void Main(string[] args)
        {
            TelemetryHandler telemetryHandler = 
                new TelemetryHandler("Application Name", "https://localhost:44392/signalr/telemetry");

            Console.WriteLine("Starting Transmitter");
            if (telemetryHandler.Connect())
            {
                Console.WriteLine("Starting Telemetry Transmission");
                while (true)
                {
                    Double randomVal = (new Random()).NextDouble();
                    
                    if (randomVal < 0.1)
                        telemetryHandler.Heartbeat("Super Application");

                    if (randomVal > 0.9)
                        telemetryHandler.Error("Shit went sideways!");

                    telemetryHandler.Send("Record", (Int32)(randomVal * 100));
                    Thread.Sleep((Int32)((new Random()).NextDouble() * 2000));
                }
            }
            else
            {
                Console.WriteLine("Failed To Start Transmitter");
                Console.ReadKey();
            }
        }
    }
}
