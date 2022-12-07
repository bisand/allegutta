// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
using System.Text.Json.Serialization;

namespace AlleGutta.Yahoo.Models;

public class ChartResponse
{
    [JsonPropertyName("result")]
    public IEnumerable<ChartResult>? Result { get; init; }

    [JsonPropertyName("error")]
    public object? Error { get; init; }
}
