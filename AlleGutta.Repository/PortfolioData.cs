using Microsoft.Data.Sqlite;

namespace AlleGutta.Repository;
public class PortfolioData
{
    private const string ConnectionString = "Data Source=hello.db";

    public void GetData(string id)
    {
        using (var connection = new SqliteConnection(ConnectionString))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                    @"
                SELECT name
                FROM user
                WHERE id = $id
            ";
            command.Parameters.AddWithValue("$id", id);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var name = reader.GetString(0);

                    Console.WriteLine($"Hello, {name}!");
                }
            }
        }
    }
    public async Task initAsync()
    {
        using (var connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = @"CREATE TABLE IF NOT EXISTS 'portfolio'(
                'id'    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
                'name'  TEXT,
                'cash'  REAL NOT NULL DEFAULT 0.0
            );";
            await command.ExecuteNonQueryAsync();
            command.CommandText = @"CREATE TABLE IF NOT EXISTS 'portfolio_positions'(
                'id'    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
                'portfolio_id'  INTEGER NOT NULL,
                'symbol'    TEXT NOT NULL,
                'shares'    INTEGER NOT NULL  DEFAULT 0,
                'avg_price' REAL NOT NULL DEFAULT 0.0,
                FOREIGN KEY('portfolio_id') REFERENCES 'portfolio'('id')
            );";
            await command.ExecuteNonQueryAsync();
        }

    }
}