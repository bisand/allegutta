using Newtonsoft.Json;

namespace AlleGutta.Nordnet.Models;

public record NordnetJsonContent<T>
{
    [JsonProperty("body")]
    public T? Body { get; init; }
}