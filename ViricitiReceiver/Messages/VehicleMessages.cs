namespace ViricitiReceiver.Messages
{
    public class VehicleStatReceived
    {
        public VehicleStat VehicleStatus { get; }

        public VehicleStatReceived(VehicleStat vehicleStatus)
        {
            VehicleStatus = vehicleStatus;
        }
    }

    public class StoreStateRequest
    {
        public StoreStateRequest()
        {
        }
    }
}
