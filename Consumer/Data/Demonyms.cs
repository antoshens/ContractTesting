using System.Text.Json.Serialization;

namespace Consumer.Data
{
    public class Demonyms
    {
        [JsonPropertyName("fra")]
        public FrenchDemonym? French { get; set; }
        [JsonPropertyName("eng")]
        public EnglishDemonym? English { get; set; }
    }

    public abstract class DemonymBase
    {
        [JsonPropertyName("f")]
        public string F { get; set; }

        [JsonPropertyName("m")]
        public string M { get; set; }
    }

    public class FrenchDemonym : DemonymBase
    {
    }

    public class EnglishDemonym : DemonymBase
    {
    }
}