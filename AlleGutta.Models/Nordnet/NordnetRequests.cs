namespace AlleGutta.Models.Nordnet;

public record NordnetRequest(string relative_url, string method);
public record NordnetRequestBatch(NordnetRequest[] batch);
public record NordnetRequestStringBatch(string batch);
