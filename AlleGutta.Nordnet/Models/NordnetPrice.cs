public record NordnetPrice
{
    public NordnetCurrency currency { get; init; }
    public decimal value { get; init; }
}
