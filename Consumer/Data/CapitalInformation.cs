using System.Text.Json.Serialization;

namespace Consumer.Data
{
    public class CapitalInformation
    {
        [JsonPropertyName("latlng")]
        private string[] Latlng { get; set; }
    }
}