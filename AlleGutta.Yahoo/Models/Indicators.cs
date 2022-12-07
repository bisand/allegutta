using System.Text.Json.Serialization;

namespace AlleGutta.Yahoo.Models;

public class Indicators
{
    [JsonPropertyName("quote")]
    public IEnumerable<Quote>? Quote { get; init; }
}
