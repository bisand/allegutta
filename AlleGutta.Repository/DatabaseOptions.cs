namespace AlleGutta.Repository
{
    public record DatabaseOptions
    {
        public const string SectionName = "Database";
        public string ConnectionString { get; init; } = string.Empty;
    }
}