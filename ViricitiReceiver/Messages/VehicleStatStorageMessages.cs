namespace ViricitiReceiver.Messages
{
    public class VehicleStatStoreRequest
    {
        public VehicleStat VehicleStatus { get; }

        public VehicleStatStoreRequest(VehicleStat vehicleStatus)
        {
            VehicleStatus = vehicleStatus;
        }
    }
}
