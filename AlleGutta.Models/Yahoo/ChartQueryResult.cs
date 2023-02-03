using System.Text.Json.Serialization;

namespace AlleGutta.Models.Yahoo;

public class ChartQueryResult
{
    [JsonPropertyName("chart")]
    public ChartResponse? Chart { get; init; }
}