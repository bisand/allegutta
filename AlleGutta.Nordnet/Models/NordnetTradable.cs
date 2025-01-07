using System.Text.Json.Serialization;

namespace AlleGutta.Nordnet.Models;

public record NordnetTradable
{
    [JsonPropertyName("market_id")]
    public int MarketId { get; init; }
    [JsonPropertyName("tick_size_id")]
    public int TickSizeId { get; init; }
    [JsonPropertyName("display_order")]
    public int DisplayOrder { get; init; }
    [JsonPropertyName("lot_size")]
    public decimal LotSize { get; init; }
    [JsonPropertyName("mic")]
    public string? Mic { get; init; }
    [JsonPropertyName("identifier")]
    public string? Identifier { get; init; }
}