public record NordNetConfig(string Url, string Username, string Password);

public record NordnetBatchData
{
    public NordnetAccountInfo? AccountInfo { get; init; }
    public NordnetPosition[]? Positions { get; init; }
    public DateTime? CacheUpdated { get; init; }
}

public record NordnetAccountInfo
{
    public NordnetPrice? account_credit { get; init; }
    public NordnetPrice? collateral { get; init; }
    public NordnetPrice? pawn_value { get; init; }
    public NordnetPrice? trading_power { get; init; }
    public NordnetPrice? loan_limit { get; init; }
    public NordnetPrice? forward_sum { get; init; }
    public NordnetPrice? future_sum { get; init; }
    public NordnetCurrency? account_currency { get; init; }
    public NordnetPrice? interest { get; init; }
    public NordnetPrice? account_sum { get; init; }
    public NordnetPrice? unrealized_future_profit_loss { get; init; }
    public NordnetPrice? own_capital { get; init; }
    public NordnetPrice? own_capital_morning { get; init; }
    public NordnetPrice? full_marketvalue { get; init; }
    public string? account_code { get; init; }
    public string? registration_date { get; init; }
    public int accno { get; init; }
    public int accid { get; init; }
}

public enum NordnetCurrency
{
    NOK
}

public record NordnetPrice
{
    public NordnetCurrency currency { get; init; }
    public int value { get; init; }
}

public record NordnetPosition
{
    public int accno { get; init; }
    public int accid { get; init; }
    public NordnetInstrument? instrument { get; init; }
    public NordnetPrice? main_market_price { get; init; }
    public NordnetPrice? morning_price { get; init; }
    public int qty { get; init; }
    public int pawn_percent { get; init; }
    public NordnetPrice? market_value_acc { get; init; }
    public NordnetPrice? market_value { get; init; }
    public NordnetPrice? acq_price { get; init; }
    public NordnetPrice? acq_price_acc { get; init; }
    public bool is_custom_gav { get; init; }
    public int margin_percent { get; init; }
}

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
    public int multiplier { get; init; }
    public int pawn_percentage { get; init; }
    public int margin_percentage { get; init; }
    public string? symbol { get; init; }
    public string? isin_code { get; init; }
    public string? sector { get; init; }
    public string? sector_group { get; init; }
    public string? name { get; init; }
}

public record NordnetTradable
{
    public int market_id { get; init; }
    public int tick_size_id { get; init; }
    public int display_order { get; init; }
    public int lot_size { get; init; }
    public string? mic { get; init; }
    public string? identifier { get; init; }
}
