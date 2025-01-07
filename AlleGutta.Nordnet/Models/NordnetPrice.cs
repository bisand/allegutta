using System.Text.Json.Serialization;

namespace AlleGutta.Nordnet.Models;

public record NordnetPrice
{
    [JsonPropertyName("currency")]
    public NordnetCurrency Currency { get; init; }
    [JsonPropertyName("value")]
    public decimal Value { get; init; }
}