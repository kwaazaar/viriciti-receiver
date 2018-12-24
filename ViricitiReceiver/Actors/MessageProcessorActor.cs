using Akka.Actor;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using ViricitiReceiver.Messages;

namespace ViricitiReceiver.Actors
{
    public class MessageProcessorActor: ReceiveActor
    {
        static readonly Dictionary<string, Action<JToken, VehicleStat>> whitelist = new Dictionary<string, Action<JToken, VehicleStat>>
            {
                { "dashboard.RemainingRange", (token, vs) => vs.RemainingRange = token.Value<float>() },
                { "dashboard.TachographVehicleSpeed", (token, vs) => vs.TachoSpeed = token.Value<float>() },
                { "dashboard.PowerConsumption", (token, vs) => vs.PowerConsumption = token.Value<float>() },
                { "ivh.gps_speed", (token, vs) => vs.GpsSpeed = token.Value<float>() },
                { "ivh.gps_position", (token, vs) => vs.GpsPos = token.Value<string>().Replace('|', ',') },
                { "ivh.altitude", (token, vs) => vs.GpsAltitude = token.Value<float>() },
                { "ivh.satellites", (token, vs) => vs.GpsSatCount = token.Value<int>() },
                { "engine.MotorTorque", (token, vs) => vs.EngineTorque = token.Value<float>() },
                { "engine.MotorSpeed", (token, vs) => vs.EngineRpm = token.Value<float>() },
                { "battery.StateOfCharge", (token, vs) => vs.StateOfCharge = token.Value<float>() },
                { "battery.TotalCurrent", (token, vs) => vs.TotalCurrent = token.Value<float>() },
                { "battery.TotalVoltage", (token, vs) => vs.TotalVoltage = token.Value<float>() },
            };

        private readonly IActorRef _vehicleStatProcessorActor;

        public MessageProcessorActor(IActorRef vehicleStatProcessorActor)
        {
            _vehicleStatProcessorActor = vehicleStatProcessorActor;

            Receive<MessageReceived>((msg) => ProcessMessage(msg));
        }

        private bool ProcessMessage(MessageReceived msg)
        {
            var jObj = JObject.Parse(msg.Message);
            var property = jObj["label"].Value<string>();

            if (whitelist.TryGetValue(property, out Action<JToken, VehicleStat> mapper)) // case-sensitive
            {
                // Initialize a new vehiclestat
                var vs = new VehicleStat
                {
                    Fleet = jObj["asset"]["fleet"].Value<string>(),
                    Vehicle = jObj["asset"]["name"].Value<string>(),
                    Timestamp = jObj["time"].Value<long>(),
                };

                // Set value to the appropriate property
                mapper(jObj["value"], vs);

                _vehicleStatProcessorActor.Tell(new VehicleStatMessage(vs));
            }

            return true;
        }

    }
}
