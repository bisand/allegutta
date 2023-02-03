using System.Text.Json.Serialization;

namespace AlleGutta.Models.Yahoo;

public class Indicators
{
    [JsonPropertyName("quote")]
    public IEnumerable<Quote>? Quote { get; init; }
}
