using System.Text.Json.Serialization;

namespace ContractTesting_Producer.Models
{
    public class Maps
    {
        [JsonPropertyName("googleMaps")]
        public string GoogleMaps { get; set; }

        [JsonPropertyName("openStreetMaps")]
        public string OpenStreetMaps { get; set; }
    }
}