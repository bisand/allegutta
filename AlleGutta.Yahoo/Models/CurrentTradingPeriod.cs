using System.Text.Json.Serialization;

namespace AlleGutta.Yahoo.Models;

public class CurrentTradingPeriod
{
    [JsonPropertyName("pre")]
    public TradingPeriod? Pre { get; init; }

    [JsonPropertyName("regular")]
    public TradingPeriod? Regular { get; init; }

    [JsonPropertyName("post")]
    public TradingPeriod? Post { get; init; }
}
