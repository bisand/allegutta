using Newtonsoft.Json;

namespace AlleGutta.Nordnet.Models;

public record NordnetPosition
{
    [JsonProperty("accno")]
    public int AccNo { get; init; }
    [JsonProperty("accid")]
    public int AccId { get; init; }
    [JsonProperty("instrument")]
    public NordnetInstrument? Instrument { get; init; }
    [JsonProperty("main_market_price")]
    public NordnetPrice? MainMarketPrice { get; init; }
    [JsonProperty("morning_price")]
    public NordnetPrice? MorningPrice { get; init; }
    [JsonProperty("qty")]
    public decimal Qty { get; init; }
    [JsonProperty("pawn_percent")]
    public decimal PawnPercent { get; init; }
    [JsonProperty("market_value_acc")]
    public NordnetPrice? MarketValueAcc { get; init; }
    [JsonProperty("market_value")]
    public NordnetPrice? MarketValue { get; init; }
    [JsonProperty("acq_price")]
    public NordnetPrice? AcqPrice { get; init; }
    [JsonProperty("acq_price_acc")]
    public NordnetPrice? AcqPriceAcc { get; init; }
    [JsonProperty("is_custom_gav")]
    public bool IsCustomGav { get; init; }
    [JsonProperty("margin_percent")]
    public decimal MarginPercent { get; init; }
}