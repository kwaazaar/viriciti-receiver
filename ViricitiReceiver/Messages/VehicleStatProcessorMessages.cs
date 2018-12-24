namespace ViricitiReceiver.Messages
{
    public class VehicleStatMessage
    {
        public VehicleStat VehicleStatus { get; }

        public VehicleStatMessage(VehicleStat vehicleStatus)
        {
            VehicleStatus = vehicleStatus;
        }
    }
}
