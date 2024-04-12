using System.Text.Json.Serialization;
using System.Transactions;

namespace ContractTesting_Producer.Models
{
    public class CountryName : Translation
    {
        [JsonPropertyName("nativeName")]
        public Dictionary<string, Translation>? NativeName { get; set; }
    }
}