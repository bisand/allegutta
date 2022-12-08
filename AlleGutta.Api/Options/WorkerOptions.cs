public record WorkerOptions
{
    public const string SectionName = "Worker";

    public TimeSpan ExecutionInterval { get; init; } = new(0, 0, 1);
    public TimeSpan RunIntervalNordnet { get; init; } = new(5, 0, 0);
    public TimeSpan RunIntervalMarkedData { get; init; } = new(0, 0, 10);
}