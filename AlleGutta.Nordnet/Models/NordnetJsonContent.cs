using System.Text.Json.Serialization;

namespace AlleGutta.Nordnet.Models;

public record NordnetJsonContent<T>
{
    [JsonPropertyName("body")]
    public T? Body { get; init; }
}