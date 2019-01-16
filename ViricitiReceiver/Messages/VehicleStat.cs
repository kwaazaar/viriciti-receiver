using Newtonsoft.Json;
using System;
using System.Text;

namespace ViricitiReceiver.Messages
{
    public class VehicleStat
    {
        [JsonProperty(PropertyName = "fleet", Required = Required.Always)]
        public string Fleet { get; set; }
        [JsonProperty(PropertyName = "vehicle", Required = Required.Always)]
        public string Vehicle { get; set; }
        [JsonProperty(PropertyName = "timestamp", Required = Required.Always)]
        public long Timestamp { get; set; }


        [JsonProperty(PropertyName = "remainingRange", NullValueHandling = NullValueHandling.Ignore)]
        public float? RemainingRange { get; set; }
        [JsonProperty(PropertyName = "estRangeInService", NullValueHandling = NullValueHandling.Ignore)]
        public float? EstRangeInService { get; set; }
        [JsonProperty(PropertyName = "powerConsumption", NullValueHandling = NullValueHandling.Ignore)]
        public float? PowerConsumption { get; set; }
        [JsonProperty(PropertyName = "stateOfCharge", NullValueHandling = NullValueHandling.Ignore)]
        public float? StateOfCharge { get; set; }
        [JsonProperty(PropertyName = "batSoC", NullValueHandling = NullValueHandling.Ignore)]
        public float? BatSoc { get; set; }
        [JsonProperty(PropertyName = "oppChargeSoC", NullValueHandling = NullValueHandling.Ignore)]
        public float? OpChargeSoc { get; set; }
        [JsonProperty(PropertyName = "filteredSoC", NullValueHandling = NullValueHandling.Ignore)]
        public float? FilteredSoc { get; set; }


        [JsonProperty(PropertyName = "totalCurrent", NullValueHandling = NullValueHandling.Ignore)]
        public float? TotalCurrent { get; set; }
        [JsonProperty(PropertyName = "totalVoltage", NullValueHandling = NullValueHandling.Ignore)]
        public float? TotalVoltage { get; set; }


        [JsonProperty(PropertyName = "tachoSpeed", NullValueHandling = NullValueHandling.Ignore)]
        public float? TachoSpeed { get; set; }

        [JsonProperty(PropertyName = "gpsSpeed", NullValueHandling = NullValueHandling.Ignore)]
        public float? GpsSpeed { get; set; }
        [JsonProperty(PropertyName = "gpsPos", NullValueHandling = NullValueHandling.Ignore)]
        public string GpsPos { get; set; }
        [JsonProperty(PropertyName = "gpsAltitude", NullValueHandling = NullValueHandling.Ignore)]
        public float? GpsAltitude { get; set; }
        [JsonProperty(PropertyName = "gpsSatCount", NullValueHandling = NullValueHandling.Ignore)]
        public int? GpsSatCount { get; set; }


        [JsonProperty(PropertyName = "engineTorque", NullValueHandling = NullValueHandling.Ignore)]
        public float? EngineTorque { get; set; }
        [JsonProperty(PropertyName = "engineRpm", NullValueHandling = NullValueHandling.Ignore)]
        public float? EngineRpm { get; set; }

        /// <summary>
        /// A singular field represention of the 'key' of this vehicle (fleet+vehicle)
        /// </summary>
        [JsonIgnore()]
        public string Key
        {
            get
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(this.Fleet + this.Vehicle));
            }
        }
    }
}
