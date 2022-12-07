namespace AlleGutta.Nordnet.Models;

public record NordnetBatchData
{
    public NordnetAccountInfo? AccountInfo { get; set; }
    public NordnetPosition[]? Positions { get; set; }
    public DateTime? CacheUpdated { get; set; }
}