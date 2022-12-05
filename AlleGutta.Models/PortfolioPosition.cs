namespace AlleGutta.Models;

public record PortfolioPosition
{
    public int Id { get; set; }
    public int PortfolioId { get; set; }
    public string? Symbol { get; init; }
    public int Shares { get; init; }
    public decimal AvgPrice { get; init; }
    public string? Name { get; init; }
    public decimal LastPrice { get; init; }
    public decimal ChangeToday { get; init; }
    public decimal ChangeTodayPercent { get; init; }
    public decimal PrevClose { get; init; }
    public decimal CostValue { get; init; }
    public decimal CurrentValue { get; init; }
    public decimal Return { get; init; }
    public decimal ReturnPercent { get; init; }
}