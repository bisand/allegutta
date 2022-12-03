public record NordnetInstrument
{
    public int mifid2_category { get; init; }
    public string? price_type { get; init; }
    public NordnetTradable[]? tradables { get; init; }
    public int instrument_id { get; init; }
    public string? asset_class { get; init; }
    public string? instrument_type { get; init; }
    public string? instrument_group_type { get; init; }
    public string? currency { get; init; }
    public decimal multiplier { get; init; }
    public decimal pawn_percentage { get; init; }
    public decimal margin_percentage { get; init; }
    public string? symbol { get; init; }
    public string? isin_code { get; init; }
    public string? sector { get; init; }
    public string? sector_group { get; init; }
    public string? name { get; init; }
}
