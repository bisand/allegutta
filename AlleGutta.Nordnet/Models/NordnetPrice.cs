using System.Text.Json.Serialization;

namespace AlleGutta.Nordnet.Models;

public record NordnetPrice
{
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }
    [JsonPropertyName("value")]
    public decimal Value { get; init; }
}