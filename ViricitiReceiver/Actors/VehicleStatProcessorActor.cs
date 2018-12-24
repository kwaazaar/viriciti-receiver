using Akka.Actor;
using System;
using System.Collections.Generic;
using ViricitiReceiver.Messages;

namespace ViricitiReceiver.Actors
{
    public class VehicleStatProcessorActor : ReceiveActor
    {
        private readonly IActorRef _vehicleStatStoreActor;
        private readonly IDictionary<string, IActorRef> _vehicleActorRefs = new Dictionary<string, IActorRef>();
        private readonly TimeSpan _cacheDuration;

        public VehicleStatProcessorActor(TimeSpan cacheDuration, IActorRef vehicleStatStoreActor)
        {
            _cacheDuration = cacheDuration;
            _vehicleStatStoreActor = vehicleStatStoreActor;

            Receive<VehicleStatMessage>((msg) => ProcessVehicleStatMessage(msg));
        }

        private bool ProcessVehicleStatMessage(VehicleStatMessage msg)
        {
            IActorRef vehicleActor = FindOrCreateVehicleActor(msg.VehicleStatus);
            vehicleActor.Tell(new VehicleStatReceived(msg.VehicleStatus));
            return true;
        }

        private IActorRef FindOrCreateVehicleActor(VehicleStat vs)
        {
            var key = vs.Key;
            if (_vehicleActorRefs.TryGetValue(key, out IActorRef existingVehicleActor))
            {
                return existingVehicleActor;
            }

            var props = Props.Create(() => new VehicleActor(vs.Fleet, vs.Vehicle, _cacheDuration, _vehicleStatStoreActor));
            var newVehicleActor = Context.ActorOf(props, "vehicle-" + key);
            _vehicleActorRefs.Add(key, newVehicleActor);
            return newVehicleActor;
        }
    }
}
