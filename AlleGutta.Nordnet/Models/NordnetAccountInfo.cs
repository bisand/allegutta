using System.Text.Json.Serialization;

namespace AlleGutta.Nordnet.Models;

public record NordnetAccountInfo
{
    [JsonPropertyName("account_credit")]
    public NordnetPrice? AccountCredit { get; init; }
    [JsonPropertyName("collateral")]
    public NordnetPrice? Collateral { get; init; }
    [JsonPropertyName("pawn_value")]
    public NordnetPrice? PawnValue { get; init; }
    [JsonPropertyName("trading_power")]
    public NordnetPrice? TradingPower { get; init; }
    [JsonPropertyName("loan_limit")]
    public NordnetPrice? LoanLimit { get; init; }
    [JsonPropertyName("forward_sum")]
    public NordnetPrice? ForwardSum { get; init; }
    [JsonPropertyName("future_sum")]
    public NordnetPrice? FutureSum { get; init; }
    [JsonPropertyName("account_currency")]
    public NordnetCurrency? AccountCurrency { get; init; }
    [JsonPropertyName("interest")]
    public NordnetPrice? Interest { get; init; }
    [JsonPropertyName("account_sum")]
    public NordnetPrice? AccountSum { get; init; }
    [JsonPropertyName("unrealized_future_profit_loss")]
    public NordnetPrice? UnrealizedFutureProfitLoss { get; init; }
    [JsonPropertyName("own_capital")]
    public NordnetPrice? OwnCapital { get; init; }
    [JsonPropertyName("own_capital_morning")]
    public NordnetPrice? OwnCapitalMorning { get; init; }
    [JsonPropertyName("full_marketvalue")]
    public NordnetPrice? FullMarketvalue { get; init; }
    [JsonPropertyName("account_code")]
    public string? AccountCode { get; init; }
    [JsonPropertyName("registration_date")]
    public string? RegistrationDate { get; init; }
    [JsonPropertyName("accno")]
    public int Accno { get; init; }
    [JsonPropertyName("accid")]
    public int Accid { get; init; }
}