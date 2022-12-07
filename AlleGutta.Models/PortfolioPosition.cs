namespace AlleGutta.Models;

public record PortfolioPosition
{
    public int Id { get; set; }
    public int PortfolioId { get; set; }
    public string? Symbol { get; set; }
    public int Shares { get; set; }
    public decimal AvgPrice { get; set; }
    public string? Name { get; set; }
    public decimal LastPrice { get; set; }
    public decimal ChangeToday { get; set; }
    public decimal ChangeTodayPercent { get; set; }
    public decimal PrevClose { get; set; }
    public decimal CostValue { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal Return { get; set; }
    public decimal ReturnPercent { get; set; }
}