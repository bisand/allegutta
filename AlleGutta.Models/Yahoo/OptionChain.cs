// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
using System.Text.Json.Serialization;

namespace AlleGutta.Models.Yahoo;

public record OptionChain(
    [property: JsonPropertyName("result")] IReadOnlyList<OptionResult> Result,
    [property: JsonPropertyName("error")] object Error
);
