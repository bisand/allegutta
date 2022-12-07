using System.Text.Json.Serialization;

namespace AlleGutta.Yahoo.Models;

public class ChartQueryResult
{
    [JsonPropertyName("chart")]
    public ChartResponse? Chart { get; init; }
}