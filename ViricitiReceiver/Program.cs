using Akka.Actor;
using System;
using System.Threading;
using ViricitiReceiver.Actors;
using ViricitiReceiver.Messages;

namespace ViricitiReceiver
{

    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 0)
            {
                Console.WriteLine("viricitireceiver <token>");
                return -1;
            }

            // Setup the actors
            var serverUrl = new Uri("wss://expose.viriciti.com/?token=" + args[0]); // Websocket URL to Viriciti api. Besides access-token, there's also IP-whitelisting active!
            var esBaseUrl = new Uri("http://localhost:9200/"); // ElasticSearch URL
            var system = ActorSystem.Create("VehicleStatSystem");
            Props vehicleStatStoreActorProps = Props.Create(() => new VehicleStatStoreActor(esBaseUrl));
            IActorRef vehicleStatStoreActor = system.ActorOf(vehicleStatStoreActorProps, "vehicleStatStore");
            Props vehicleStatProcessorActorProps = Props.Create(() => new VehicleStatProcessorActor(TimeSpan.FromSeconds(10), vehicleStatStoreActor)); // Store once every x seconds
            IActorRef vehicleStatProcessorActor = system.ActorOf(vehicleStatProcessorActorProps, "vehicleStatProcessor");
            Props messageProcessorActorProps = Props.Create(() => new MessageProcessorActor(vehicleStatProcessorActor));
            IActorRef messageProcessorActor = system.ActorOf(messageProcessorActorProps, "messageProcessor");
            Props viricitiReaderActorProps = Props.Create(() => new ViricitiWebSocketReaderActor(serverUrl, messageProcessorActor));
            IActorRef viricitiReaderActor = system.ActorOf(viricitiReaderActorProps, "viricitiReader");

            // Make the 'root' actor start and wait for Ctrl-C to be pressed (or the process to be killed)
            viricitiReaderActor.Tell(new StartProcessing());
            var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                cancellationTokenSource.Cancel();
            };
            Console.WriteLine("Started. Press Ctrl-C to exit...");
            while (!cancellationTokenSource.IsCancellationRequested)
                Thread.Sleep(250);

            // Shutdown everything
            viricitiReaderActor.Tell(new StopProcessing());
            system.Terminate().GetAwaiter().GetResult(); // Terminate gracefully (you may get errors for already scheduled messages)

            return 0;
        }
    }
}
