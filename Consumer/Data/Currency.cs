using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Consumer.Data
{
    public class Currency
    {
        [JsonPropertyName("name")]
        [Required]
        public string? Name { get; set; }

        [JsonPropertyName("symbol")]
        [Required]
        public string? Symbol { get; set; }
    }
}
