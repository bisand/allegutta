public record NordnetTradable
{
    public int market_id { get; init; }
    public int tick_size_id { get; init; }
    public int display_order { get; init; }
    public decimal lot_size { get; init; }
    public string? mic { get; init; }
    public string? identifier { get; init; }
}
