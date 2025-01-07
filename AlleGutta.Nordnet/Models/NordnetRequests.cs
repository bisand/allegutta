using System.Text.Json.Serialization;

namespace AlleGutta.Nordnet.Models;

public record NordnetRequest([property: JsonPropertyName("relative_url")] string RelativeUrl, [property: JsonPropertyName("method")] string Method);
public record NordnetRequestBatch([property: JsonPropertyName("batch")] NordnetRequest[] Batch);
public record NordnetRequestStringBatch([property: JsonPropertyName("batch")] string Batch);
