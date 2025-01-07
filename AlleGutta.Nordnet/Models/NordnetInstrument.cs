using System.Text.Json.Serialization;

namespace AlleGutta.Nordnet.Models;

public record NordnetInstrument
{
    [JsonPropertyName("mifid2_category")]
    public int Mifid2Category { get; init; }
    [JsonPropertyName("price_type")]
    public string? PriceType { get; init; }
    [JsonPropertyName("tradables")]
    public NordnetTradable[]? Tradables { get; init; }
    [JsonPropertyName("instrument_id")]
    public int InstrumentId { get; init; }
    [JsonPropertyName("asset_class")]
    public string? AssetClass { get; init; }
    [JsonPropertyName("instrument_type")]
    public string? InstrumentType { get; init; }
    [JsonPropertyName("instrument_group_type")]
    public string? InstrumentGroupType { get; init; }
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }
    [JsonPropertyName("multiplier")]
    public decimal Multiplier { get; init; }
    [JsonPropertyName("pawn_percentage")]
    public decimal PawnPercentage { get; init; }
    [JsonPropertyName("margin_percentage")]
    public decimal MarginPercentage { get; init; }
    [JsonPropertyName("symbol")]
    public string? Symbol { get; init; }
    [JsonPropertyName("isin_code")]
    public string? IsinCode { get; init; }
    [JsonPropertyName("sector")]
    public string? Sector { get; init; }
    [JsonPropertyName("sector_group")]
    public string? SectorGroup { get; init; }
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}