// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
using System.Text.Json.Serialization;

namespace AlleGutta.Models.Yahoo;

public class Quote
{
    [JsonPropertyName("volume")]
    public IEnumerable<int?>? Volume { get; init; }

    [JsonPropertyName("low")]
    public IEnumerable<double?>? Low { get; init; }

    [JsonPropertyName("high")]
    public IEnumerable<double?>? High { get; init; }

    [JsonPropertyName("close")]
    public IEnumerable<double?>? Close { get; init; }

    [JsonPropertyName("open")]
    public IEnumerable<double?>? Open { get; init; }
}
