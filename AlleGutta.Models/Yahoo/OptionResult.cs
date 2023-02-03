// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
using System.Text.Json.Serialization;

namespace AlleGutta.Models.Yahoo;

public record OptionResult(
[property: JsonPropertyName("underlyingSymbol")] string UnderlyingSymbol,
[property: JsonPropertyName("expirationDates")] IReadOnlyList<object> ExpirationDates,
[property: JsonPropertyName("strikes")] IReadOnlyList<object> Strikes,
[property: JsonPropertyName("hasMiniOptions")] bool HasMiniOptions,
[property: JsonPropertyName("quote")] OptionQuote Quote,
[property: JsonPropertyName("options")] IReadOnlyList<object> Options
);
