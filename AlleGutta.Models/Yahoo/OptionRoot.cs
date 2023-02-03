// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
using System.Text.Json.Serialization;

namespace AlleGutta.Models.Yahoo;

public record OptionRoot(
[property: JsonPropertyName("optionChain")] OptionChain OptionChain
);
