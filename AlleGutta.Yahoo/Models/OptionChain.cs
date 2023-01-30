// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
using System.Text.Json.Serialization;
using AlleGutta.Yahoo.Models;

namespace AlleGutta.Yahoo.Models;

public record OptionChain(
    [property: JsonPropertyName("result")] IReadOnlyList<OptionResult> Result,
    [property: JsonPropertyName("error")] object Error
);
