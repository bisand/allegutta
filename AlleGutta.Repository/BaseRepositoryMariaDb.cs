using System.Data.Common;
using AlleGutta.Common;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace AlleGutta.Repository;

public class BaseRepositoryMariaDb
{
    protected readonly ILogger<PortfolioRepositoryMariaDb> _logger;
    protected readonly DatabaseOptionsMariaDb _options;
    protected readonly MySqlConnectionStringBuilder _builder;

    public string ConnectionString { get; }

    public BaseRepositoryMariaDb(IOptions<DatabaseOptionsMariaDb> options, ILogger<PortfolioRepositoryMariaDb> logger)
    {
        _logger = logger;
        _options = options.Value;
        var root = Directory.GetCurrentDirectory();
        var dotenv = Path.Combine(root, "../.env");
        DotEnv.Load(dotenv);

        if (new[] { "MYSQL_PASSWORD" }.Any(x => Environment.GetEnvironmentVariable(x) is null))
            throw new ArgumentException("Missing MariaDB password. Use environment variables: MYSQL_PASSWORD");

        var mariaDbPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? string.Empty;
        _builder = new MySqlConnectionStringBuilder
        {
            Server = _options.Server,
            Database = _options.Database,
            UserID = _options.UserID,
            Password = mariaDbPassword,
            SslMode = _options.SslMode,
            ConvertZeroDateTime = true,
        };
        ConnectionString = _builder.ConnectionString;
    }


    public async IAsyncEnumerable<T> GetDataAsync<T>(string sql, IEnumerable<DbParameter> parameters)
    {
        using var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddRange(parameters.ToArray());

        using var reader = await command.ExecuteReaderAsync();
        var parser = reader.GetRowParser<T>(typeof(T));

        while (await reader.ReadAsync())
        {
            yield return parser(reader);
        }
    }
}
