// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
using System.Text.Json.Serialization;

namespace AlleGutta.Yahoo.Models;

public class Meta
{
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    [JsonPropertyName("symbol")]
    public string? Symbol { get; init; }

    [JsonPropertyName("exchangeName")]
    public string? ExchangeName { get; init; }

    [JsonPropertyName("instrumentType")]
    public string? InstrumentType { get; init; }

    [JsonPropertyName("firstTradeDate")]
    public DateTime? FirstTradeDate { get; init; }

    [JsonPropertyName("regularMarketTime")]
    public DateTime? RegularMarketTime { get; init; }

    [JsonPropertyName("gmtoffset")]
    public int? Gmtoffset { get; init; }

    [JsonPropertyName("timezone")]
    public string? Timezone { get; init; }

    [JsonPropertyName("exchangeTimezoneName")]
    public string? ExchangeTimezoneName { get; init; }

    [JsonPropertyName("regularMarketPrice")]
    public double? RegularMarketPrice { get; init; }

    [JsonPropertyName("chartPreviousClose")]
    public double? ChartPreviousClose { get; init; }

    [JsonPropertyName("previousClose")]
    public double? PreviousClose { get; init; }

    [JsonPropertyName("scale")]
    public int? Scale { get; init; }

    [JsonPropertyName("priceHint")]
    public int? PriceHint { get; init; }

    [JsonPropertyName("currentTradingPeriod")]
    public CurrentTradingPeriod? CurrentTradingPeriod { get; init; }

    [JsonPropertyName("tradingPeriods")]
    public IEnumerable<TradingPeriod>? TradingPeriods { get; init; }

    [JsonPropertyName("dataGranularity")]
    public string? DataGranularity { get; init; }

    [JsonPropertyName("range")]
    public string? Range { get; init; }

    [JsonPropertyName("validRanges")]
    public IEnumerable<string>? ValidRanges { get; init; }
}
