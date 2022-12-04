namespace AlleGutta.Models.Nordnet;

public record NordnetPosition
{
    public int accno { get; init; }
    public int accid { get; init; }
    public NordnetInstrument? instrument { get; init; }
    public NordnetPrice? main_market_price { get; init; }
    public NordnetPrice? morning_price { get; init; }
    public decimal qty { get; init; }
    public decimal pawn_percent { get; init; }
    public NordnetPrice? market_value_acc { get; init; }
    public NordnetPrice? market_value { get; init; }
    public NordnetPrice? acq_price { get; init; }
    public NordnetPrice? acq_price_acc { get; init; }
    public bool is_custom_gav { get; init; }
    public decimal margin_percent { get; init; }
}