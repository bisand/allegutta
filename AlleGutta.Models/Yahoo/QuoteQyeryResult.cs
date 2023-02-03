// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
using System.Text.Json.Serialization;

namespace AlleGutta.Models.Yahoo;

public record QuoteQyeryResult
{
    [JsonPropertyName("quoteResponse")]
    public QuoteResponse? QuoteResponse { get; init; }
}
