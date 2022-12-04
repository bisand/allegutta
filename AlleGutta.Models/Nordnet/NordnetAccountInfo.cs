namespace AlleGutta.Models.Nordnet;

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