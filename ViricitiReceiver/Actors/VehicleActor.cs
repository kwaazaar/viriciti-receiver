using System;
using Akka.Actor;
using ViricitiReceiver.Messages;

namespace ViricitiReceiver.Actors
{
    public class VehicleActor : ReceiveActor
    {
        private readonly IActorRef _vehicleStatStoreActor;
        private readonly VehicleStat _vehicleStat;
        private long _lastStoredTimestamp;

        public VehicleActor(string fleet, string vehicle, TimeSpan cacheDuration, IActorRef vehicleStatStoreActor)
        {
            _vehicleStat = new VehicleStat()
            {
                Fleet = fleet,
                Vehicle = vehicle,
                Timestamp = default(long),
            };

            _vehicleStatStoreActor = vehicleStatStoreActor;

            Receive<VehicleStatReceived>((msg) => ProcessVehicleStatReceived(msg));
            Receive<StoreStateRequest>((msg) => StoreStateRequest(msg));

            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(0), cacheDuration, Self, new StoreStateRequest(), ActorRefs.NoSender);
        }

        private void StoreStateRequest(StoreStateRequest msg)
        {
            if (_lastStoredTimestamp != _vehicleStat.Timestamp) // Only store when there are differences
            {
                // Only store when updated data was received
                _vehicleStatStoreActor.Tell(new VehicleStatStoreRequest(_vehicleStat));
                _lastStoredTimestamp = _vehicleStat.Timestamp;
            }
        }

        private bool ProcessVehicleStatReceived(VehicleStatReceived msg)
        {
            var source = msg.VehicleStatus;
            var updated = false;

            WhenDiff(source.EngineRpm, _vehicleStat.EngineRpm, ref updated, (s) => _vehicleStat.EngineRpm = s);
            WhenDiff(source.EngineTorque, _vehicleStat.EngineTorque, ref updated, (s) => _vehicleStat.EngineTorque = s);
            WhenDiff(source.GpsAltitude, _vehicleStat.GpsAltitude, ref updated, (s) => _vehicleStat.GpsAltitude = s);
            WhenDiff(source.GpsPos, _vehicleStat.GpsPos, ref updated, (s) => _vehicleStat.GpsPos = s);
            WhenDiff(source.GpsSatCount, _vehicleStat.GpsSatCount, ref updated, (s) => _vehicleStat.GpsSatCount = s);
            WhenDiff(source.GpsSpeed, _vehicleStat.GpsSpeed, ref updated, (s) => _vehicleStat.GpsSpeed = s);
            WhenDiff(source.PowerConsumption, _vehicleStat.PowerConsumption, ref updated, (s) => _vehicleStat.PowerConsumption = s);
            WhenDiff(source.RemainingRange, _vehicleStat.RemainingRange, ref updated, (s) => _vehicleStat.RemainingRange = s);
            WhenDiff(source.StateOfCharge, _vehicleStat.StateOfCharge, ref updated, (s) => _vehicleStat.StateOfCharge = s);
            WhenDiff(source.TachoSpeed, _vehicleStat.TachoSpeed, ref updated, (s) => _vehicleStat.TachoSpeed = s);
            WhenDiff(source.TotalCurrent, _vehicleStat.TotalCurrent, ref updated, (s) => _vehicleStat.TotalCurrent = s);
            WhenDiff(source.TotalVoltage, _vehicleStat.TotalVoltage, ref updated, (s) => _vehicleStat.TotalVoltage = s);

            if (updated)
            {
                _vehicleStat.Timestamp = source.Timestamp;
            }

            return true;
        }

        private bool WhenDiff(float? source, float? target, ref bool updated, Action<float?> action)
        {
            bool hasDiff = (source.HasValue && source.Value != target.GetValueOrDefault());
            if (hasDiff)
            {
                action(source);
                updated = true;
            }
            return hasDiff;
        }

        private bool WhenDiff(int? source, int? target, ref bool updated, Action<int?> action)
        {
            bool hasDiff = (source.HasValue && source.Value != target.GetValueOrDefault());
            if (hasDiff)
            {
                action(source);
                updated = true;
            }
            return hasDiff;
        }

        private bool WhenDiff(string source, string target, ref bool updated, Action<string> action)
        {
            bool hasDiff = (!String.IsNullOrWhiteSpace(source) && (target == null || source != target));
            if (hasDiff)
            {
                action(source);
                updated = true;
            }
            return hasDiff;
        }
    }
}
