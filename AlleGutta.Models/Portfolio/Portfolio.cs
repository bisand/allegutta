namespace AlleGutta.Models.Portfolio;

public record Portfolio
{
    public int Id { get; set; }
    public string? AccountNo { get; set; }
    public string? Name { get; set; }
    public decimal Ath { get; set; }
    public DateTime AthDate { get; set; }
    public decimal Cash { get; set; }
    public decimal ChangeTodayTotal { get; set; }
    public decimal ChangeTodayPercent { get; set; }
    public decimal ChangeTotal { get; set; }
    public decimal ChangeTotalPercent { get; set; }
    public decimal CostValue { get; set; }
    public decimal Equity { get; set; }
    public decimal MarketValue { get; set; }
    public decimal MarketValuePrev { get; set; }
    public decimal MarketValueMax { get; set; }
    public decimal MarketValueMin { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateModified { get; set; }
    public IEnumerable<PortfolioPosition>? Positions { get; set; }
}
