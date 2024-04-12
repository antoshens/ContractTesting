using System.Text.Json.Serialization;

namespace ContractTesting_Producer.Models
{
    public class Idd
    {
        [JsonPropertyName("root")]
        public string Root { get; set; }

        [JsonPropertyName("suffixes")]
        public string[] Suffixes { get; set; }
    }
}