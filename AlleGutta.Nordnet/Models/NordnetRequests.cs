using Newtonsoft.Json;

namespace AlleGutta.Nordnet.Models;

public record NordnetRequest([property: JsonProperty("relative_url")] string RelativeUrl, [property: JsonProperty("method")] string Method);
public record NordnetRequestBatch([property: JsonProperty("batch")] NordnetRequest[] Batch);
public record NordnetRequestStringBatch([property: JsonProperty("batch")] string Batch);
