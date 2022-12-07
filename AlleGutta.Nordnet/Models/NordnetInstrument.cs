using Newtonsoft.Json;

namespace AlleGutta.Nordnet.Models;

public record NordnetInstrument
{
    [JsonProperty("mifid2_category")]
    public int Mifid2Category { get; init; }
    [JsonProperty("price_type")]
    public string? PriceType { get; init; }
    [JsonProperty("tradables")]
    public NordnetTradable[]? Tradables { get; init; }
    [JsonProperty("instrument_id")]
    public int InstrumentId { get; init; }
    [JsonProperty("asset_class")]
    public string? AssetClass { get; init; }
    [JsonProperty("instrument_type")]
    public string? InstrumentType { get; init; }
    [JsonProperty("instrument_group_type")]
    public string? InstrumentGroupType { get; init; }
    [JsonProperty("currency")]
    public string? Currency { get; init; }
    [JsonProperty("multiplier")]
    public decimal Multiplier { get; init; }
    [JsonProperty("pawn_percentage")]
    public decimal PawnPercentage { get; init; }
    [JsonProperty("margin_percentage")]
    public decimal MarginPercentage { get; init; }
    [JsonProperty("symbol")]
    public string? Symbol { get; init; }
    [JsonProperty("isin_code")]
    public string? IsinCode { get; init; }
    [JsonProperty("sector")]
    public string? Sector { get; init; }
    [JsonProperty("sector_group")]
    public string? SectorGroup { get; init; }
    [JsonProperty("name")]
    public string? Name { get; init; }
}