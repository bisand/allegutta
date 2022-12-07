using Newtonsoft.Json;

namespace AlleGutta.Nordnet.Models;

public record NordnetPrice
{
    [JsonProperty("currency")]
    public NordnetCurrency Currency { get; init; }
    [JsonProperty("value")]
    public decimal Value { get; init; }
}