using Newtonsoft.Json;

namespace AlleGutta.Nordnet.Models;

public record NordnetTradable
{
    [JsonProperty("market_id")]
    public int MarketId { get; init; }
    [JsonProperty("tick_size_id")]
    public int TickSizeId { get; init; }
    [JsonProperty("display_order")]
    public int DisplayOrder { get; init; }
    [JsonProperty("lot_size")]
    public decimal LotSize { get; init; }
    [JsonProperty("mic")]
    public string? Mic { get; init; }
    [JsonProperty("identifier")]
    public string? Identifier { get; init; }
}