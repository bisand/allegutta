// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
using System.Text.Json.Serialization;

namespace AlleGutta.Yahoo.Models;

public record QuoteResponse
{
    [JsonPropertyName("result")]
    public IEnumerable<QuoteResult>? Result { get; init; }

    [JsonPropertyName("error")]
    public object? Error { get; set; }
}
