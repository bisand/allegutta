namespace AlleGutta.Models;

public record Portfolio
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public decimal Cash { get; init; }
    public decimal Ath { get; init; }
    public decimal Equity { get; init; }
    public decimal CostValue { get; init; }
    public decimal MarketValue { get; init; }
    public decimal MarketValuePrev { get; init; }
    public decimal MarketValueMax { get; init; }
    public decimal MarketValueMin { get; init; }
    public decimal ChangeTodayTotal { get; init; }
    public decimal ChangeTodayPercent { get; init; }
    public decimal ChangeTotal { get; init; }
    public decimal ChangeTotalPercent { get; init; }
    public PortfolioPosition[]? Positions { get; init; }
}