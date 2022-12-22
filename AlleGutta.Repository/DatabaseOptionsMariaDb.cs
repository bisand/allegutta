using MySqlConnector;

namespace AlleGutta.Repository
{
    public record DatabaseOptionsMariaDb
    {
        public const string SectionName = "Database.MariaDb";
        public string Server { get; init; } = string.Empty;
        public string Database { get; init; } = string.Empty;
        public string UserID { get; init; } = string.Empty;
        public MySqlSslMode SslMode { get; init; } = MySqlSslMode.Required;
    }
}