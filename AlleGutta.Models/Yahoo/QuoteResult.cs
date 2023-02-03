using System.Text.Json.Serialization;

namespace AlleGutta.Models.Yahoo;

public record QuoteResult
{
    [JsonPropertyName("language")]
    public string? Language { get; init; }

    [JsonPropertyName("region")]
    public string? Region { get; init; }

    [JsonPropertyName("quoteType")]
    public string? QuoteType { get; init; }

    [JsonPropertyName("typeDisp")]
    public string? TypeDisp { get; init; }

    [JsonPropertyName("quoteSourceName")]
    public string? QuoteSourceName { get; init; }

    [JsonPropertyName("triggerable")]
    public bool? Triggerable { get; init; }

    [JsonPropertyName("customPriceAlertConfidence")]
    public string? CustomPriceAlertConfidence { get; init; }

    [JsonPropertyName("marketState")]
    public string? MarketState { get; init; }

    [JsonPropertyName("regularMarketPrice")]
    public decimal? RegularMarketPrice { get; init; }

    [JsonPropertyName("regularMarketChangePercent")]
    public decimal? RegularMarketChangePercent { get; init; }

    [JsonPropertyName("longName")]
    public string? LongName { get; init; }

    [JsonPropertyName("market")]
    public string? Market { get; init; }

    [JsonPropertyName("shortName")]
    public string? ShortName { get; init; }

    [JsonPropertyName("gmtOffSetMilliseconds")]
    public int? GmtOffSetMilliseconds { get; init; }

    [JsonPropertyName("exchangeTimezoneName")]
    public string? ExchangeTimezoneName { get; init; }

    [JsonPropertyName("exchange")]
    public string? Exchange { get; init; }

    [JsonPropertyName("esgPopulated")]
    public bool? EsgPopulated { get; init; }

    [JsonPropertyName("exchangeTimezoneShortName")]
    public string? ExchangeTimezoneShortName { get; init; }

    [JsonPropertyName("regularMarketChange")]
    public decimal? RegularMarketChange { get; init; }

    [JsonPropertyName("regularMarketTime")]
    public DateTime? RegularMarketTime { get; init; }

    [JsonPropertyName("regularMarketDayHigh")]
    public decimal? RegularMarketDayHigh { get; init; }

    [JsonPropertyName("regularMarketDayRange")]
    public string? RegularMarketDayRange { get; init; }

    [JsonPropertyName("regularMarketDayLow")]
    public decimal? RegularMarketDayLow { get; init; }

    [JsonPropertyName("regularMarketVolume")]
    public int? RegularMarketVolume { get; init; }

    [JsonPropertyName("regularMarketPreviousClose")]
    public decimal? RegularMarketPreviousClose { get; init; }

    [JsonPropertyName("fullExchangeName")]
    public string? FullExchangeName { get; init; }

    [JsonPropertyName("fiftyTwoWeekLowChange")]
    public decimal? FiftyTwoWeekLowChange { get; init; }

    [JsonPropertyName("fiftyTwoWeekLowChangePercent")]
    public decimal? FiftyTwoWeekLowChangePercent { get; init; }

    [JsonPropertyName("fiftyTwoWeekRange")]
    public string? FiftyTwoWeekRange { get; init; }

    [JsonPropertyName("fiftyTwoWeekHighChange")]
    public decimal? FiftyTwoWeekHighChange { get; init; }

    [JsonPropertyName("fiftyTwoWeekHighChangePercent")]
    public decimal? FiftyTwoWeekHighChangePercent { get; init; }

    [JsonPropertyName("fiftyTwoWeekLow")]
    public decimal? FiftyTwoWeekLow { get; init; }

    [JsonPropertyName("fiftyTwoWeekHigh")]
    public decimal? FiftyTwoWeekHigh { get; init; }

    [JsonPropertyName("tradeable")]
    public bool? Tradeable { get; init; }

    [JsonPropertyName("cryptoTradeable")]
    public bool? CryptoTradeable { get; init; }

    [JsonPropertyName("firstTradeDateMilliseconds")]
    public long? FirstTradeDateMilliseconds { get; init; }

    [JsonPropertyName("priceHint")]
    public int? PriceHint { get; init; }

    [JsonPropertyName("sourceInterval")]
    public int? SourceInterval { get; init; }

    [JsonPropertyName("exchangeDataDelayedBy")]
    public int? ExchangeDataDelayedBy { get; init; }

    [JsonPropertyName("symbol")]
    public string? Symbol { get; init; }
}
