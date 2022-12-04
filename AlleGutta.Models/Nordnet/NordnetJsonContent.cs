namespace AlleGutta.Models.Nordnet;

public record NordnetJsonContent<T>
{
    public T? body { get; init; }
}