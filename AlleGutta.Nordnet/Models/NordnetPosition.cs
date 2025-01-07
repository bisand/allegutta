using System.Text.Json.Serialization;

namespace AlleGutta.Nordnet.Models;

public record NordnetPosition
{
    [JsonPropertyName("accno")]
    public int AccNo { get; init; }
    [JsonPropertyName("accid")]
    public int AccId { get; init; }
    [JsonPropertyName("instrument")]
    public NordnetInstrument? Instrument { get; init; }
    [JsonPropertyName("main_market_price")]
    public NordnetPrice? MainMarketPrice { get; init; }
    [JsonPropertyName("morning_price")]
    public NordnetPrice? MorningPrice { get; init; }
    [JsonPropertyName("qty")]
    public decimal Qty { get; init; }
    [JsonPropertyName("pawn_percent")]
    public decimal PawnPercent { get; init; }
    [JsonPropertyName("market_value_acc")]
    public NordnetPrice? MarketValueAcc { get; init; }
    [JsonPropertyName("market_value")]
    public NordnetPrice? MarketValue { get; init; }
    [JsonPropertyName("acq_price")]
    public NordnetPrice? AcqPrice { get; init; }
    [JsonPropertyName("acq_price_acc")]
    public NordnetPrice? AcqPriceAcc { get; init; }
    [JsonPropertyName("is_custom_gav")]
    public bool IsCustomGav { get; init; }
    [JsonPropertyName("margin_percent")]
    public decimal MarginPercent { get; init; }
}