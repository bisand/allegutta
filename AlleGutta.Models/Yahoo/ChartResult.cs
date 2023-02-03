// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
using System.Text.Json.Serialization;

namespace AlleGutta.Models.Yahoo;

public class ChartResult
{
    [JsonPropertyName("meta")]
    public Meta? Meta { get; init; }

    [JsonPropertyName("timestamp")]
    public IEnumerable<DateTime?>? Timestamp { get; init; }

    [JsonPropertyName("indicators")]
    public Indicators? Indicators { get; init; }
}
