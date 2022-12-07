using Newtonsoft.Json;

namespace AlleGutta.Nordnet.Models;

public record NordnetAccountInfo
{
    [JsonProperty("account_credit")]
    public NordnetPrice? AccountCredit { get; init; }
    [JsonProperty("collateral")]
    public NordnetPrice? Collateral { get; init; }
    [JsonProperty("pawn_value")]
    public NordnetPrice? PawnValue { get; init; }
    [JsonProperty("trading_power")]
    public NordnetPrice? TradingPower { get; init; }
    [JsonProperty("loan_limit")]
    public NordnetPrice? LoanLimit { get; init; }
    [JsonProperty("forward_sum")]
    public NordnetPrice? ForwardSum { get; init; }
    [JsonProperty("future_sum")]
    public NordnetPrice? FutureSum { get; init; }
    [JsonProperty("account_currency")]
    public NordnetCurrency? AccountCurrency { get; init; }
    [JsonProperty("interest")]
    public NordnetPrice? Interest { get; init; }
    [JsonProperty("account_sum")]
    public NordnetPrice? AccountSum { get; init; }
    [JsonProperty("unrealized_future_profit_loss")]
    public NordnetPrice? UnrealizedFutureProfitLoss { get; init; }
    [JsonProperty("own_capital")]
    public NordnetPrice? OwnCapital { get; init; }
    [JsonProperty("own_capital_morning")]
    public NordnetPrice? OwnCapitalMorning { get; init; }
    [JsonProperty("full_marketvalue")]
    public NordnetPrice? FullMarketvalue { get; init; }
    [JsonProperty("account_code")]
    public string? AccountCode { get; init; }
    [JsonProperty("registration_date")]
    public string? RegistrationDate { get; init; }
    [JsonProperty("accno")]
    public int Accno { get; init; }
    [JsonProperty("accid")]
    public int Accid { get; init; }
}