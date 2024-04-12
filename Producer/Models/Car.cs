using System.Text.Json.Serialization;

namespace ContractTesting_Producer.Models
{
    public class Car
    {
        [JsonPropertyName("signs")]
        public string[] Signs { get; set; }

        [JsonPropertyName("side")]
        public string Side { get; set; }
    }
}