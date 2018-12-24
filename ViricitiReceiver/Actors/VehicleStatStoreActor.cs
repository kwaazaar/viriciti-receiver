using Akka.Actor;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ViricitiReceiver.Messages;

namespace ViricitiReceiver.Actors
{
    public class VehicleStatStoreActor : ReceiveActor
    {
        static HttpClient httpClient = new HttpClient();

        private readonly Uri _esBaseUrl;

        public VehicleStatStoreActor(Uri esBaseUrl)
        {
            _esBaseUrl = esBaseUrl;
            httpClient.BaseAddress = esBaseUrl;

            ReceiveAsync<VehicleStatStoreRequest>((msg) => ProcessVehicleStatStoreRequest(msg));
        }

        private async Task<bool> ProcessVehicleStatStoreRequest(VehicleStatStoreRequest msg)
        {
            string index = "vehiclestat";
            var relativeUri = $"{index}/_doc/{Guid.NewGuid()}";
            var stringContent = JsonConvert.SerializeObject(msg.VehicleStatus, Formatting.None);
            var res = await httpClient.PostAsync(relativeUri, new StringContent(stringContent, Encoding.UTF8, "application/json"));
#if DEBUG
            if (!res.IsSuccessStatusCode)
            {
                Console.WriteLine($"Write failure: {res.StatusCode}" + res.Content.ReadAsStringAsync().Result);
            }
#endif
            return res.IsSuccessStatusCode;
        }
    }
}
