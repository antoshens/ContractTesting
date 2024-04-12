using System.Text.Json.Serialization;

namespace ContractTesting_Producer.Models
{
    public class Currency
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }
    }
}
