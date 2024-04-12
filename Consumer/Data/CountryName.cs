using System.Text.Json.Serialization;
using System.Transactions;

namespace Consumer.Data
{
    public class CountryName : Translation
    {
        [JsonPropertyName("nativeName")]
        public Dictionary<string, Translation>? NativeName { get; set; }
    }
}