namespace AlleGutta.Repository
{
    public record DatabaseOptionsSQLite
    {
        public const string SectionName = "Database.SQLite";
        public string ConnectionString { get; init; } = string.Empty;
    }
}