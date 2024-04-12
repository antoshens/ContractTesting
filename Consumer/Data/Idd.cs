using System.Text.Json.Serialization;

namespace Consumer.Data
{
    public class Idd
    {
        [JsonPropertyName("root")]
        public string Root { get; set; }

        [JsonPropertyName("suffixes")]
        public string[] Suffixes { get; set; }
    }
}