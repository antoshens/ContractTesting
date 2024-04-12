using System.Text.Json.Serialization;

namespace ContractTesting_Producer.Models
{
    public class CapitalInformation
    {
        [JsonPropertyName("latlng")]
        private string[] Latlng { get; set; }
    }
}