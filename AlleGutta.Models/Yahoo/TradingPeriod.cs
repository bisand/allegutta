// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
using System.Text.Json.Serialization;

namespace AlleGutta.Models.Yahoo;

public class TradingPeriod
{
    [JsonPropertyName("timezone")]
    public string? Timezone { get; init; }

    [JsonPropertyName("start")]
    public DateTime? Start { get; init; }

    [JsonPropertyName("end")]
    public DateTime? End { get; init; }

    [JsonPropertyName("gmtoffset")]
    public int? Gmtoffset { get; init; }
}
