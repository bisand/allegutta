using Microsoft.Data.Sqlite;
using Dapper;
using AlleGutta.Models.Nordnet;
using AlleGutta.Models;

namespace AlleGutta.Repository;
public class PortfolioData
{
    private readonly string _connectionString;

    public PortfolioData(string ConnectionString)
    {
        Task.Run(InitAsync);
        _connectionString = ConnectionString;
    }

    // async getPortfolioAsync(portfolioName: string): Promise<Portfolio> {
    //     const db = await this.open();
    //     const positions = await db.all<PortfolioPosition[]>(
    //         `
    //             SELECT p.id, p.name, p.cash, pp.id [pos_id], pp.symbol, pp.shares, pp.avg_price FROM portfolio p 
    //             LEFT JOIN portfolio_positions pp ON p.id = pp.portfolio_id 
    //             WHERE p.name = $name`,
    //         { $name: portfolioName });
    //     let portfolio = new Portfolio();
    //     positions.forEach((pos: any) => {
    //         if (!portfolio.name) {
    //             portfolio.name = pos.name;
    //             portfolio.cash = pos.cash;
    //             portfolio.id = pos.id;
    //         }
    //         const pp = new PortfolioPosition();
    //         pp.id = pos.pos_id;
    //         pp.symbol = pos.symbol;
    //         pp.shares = pos.shares;
    //         pp.avg_price = pos.avg_price;
    //         portfolio.positions.push(pp);
    //     });
    //     return portfolio;
    // }

    // async importPortfolioAsync(portfolio: Portfolio): Promise<void> {
    //     const db = await this.open();
    //     const lastId = await db
    //         .run(`INSERT INTO portfolio(name, cash) VALUES($name, $cash)`, {
    //             $name: portfolio.name,
    //             $cash: portfolio.cash,
    //             // })
    //             // .then((res: any) => {
    //             //     return res.lastID;
    //         });
    //     portfolio.positions.forEach(position => {
    //         db.run(
    //             `INSERT INTO portfolio_positions(portfolio_id, symbol, shares, avg_price) VALUES($portfolio_id, $symbol, $shares, $avg_price)`,
    //             {
    //                 $portfolio_id: lastId,
    //                 $symbol: position.symbol,
    //                 $shares: position.shares,
    //                 $avg_price: position.avg_price,
    //             },
    //             (err: any) => {
    //                 if (err) console.error(err);
    //             },
    //         );
    //     });
    //     await db.close();
    // }

    public async IAsyncEnumerable<Portfolio> GetPortfolioPositionsAsync(string portfolioName)
    {
        const string sql = @"
            SELECT pp.Id, pp.PortfolioId, pp.Symbol, pp.Shares, pp.AvgPrice 
            FROM PortfolioPositions pp
            JOIN Portfolio p ON p.Id = pp.PortfolioId
            WHERE p.Name = @portfolioName
        ";
        await foreach (var item in GetDataAsync<Portfolio>(sql, new[] { new SqliteParameter("@portfolioName", portfolioName) }))
        {
            yield return item;
        }
    }

    public async IAsyncEnumerable<T> GetDataAsync<T>(string sql, IEnumerable<SqliteParameter> parameters)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddRange(parameters);

        using var reader = await command.ExecuteReaderAsync();
        var parser = reader.GetRowParser<T>(typeof(T));

        while (await reader.ReadAsync())
        {
            yield return parser(reader);
        }
    }

    private async Task InitAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = @"CREATE TABLE IF NOT EXISTS 'Portfolio'(
                'Id'    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
                'Name'  TEXT,
                'Cash'  REAL NOT NULL DEFAULT 0.0
            );";
        await command.ExecuteNonQueryAsync();
        command.CommandText = @"CREATE TABLE IF NOT EXISTS 'PortfolioPositions'(
                'Id'    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
                'PortfolioId'  INTEGER NOT NULL,
                'Symbol'    TEXT NOT NULL,
                'Shares'    INTEGER NOT NULL  DEFAULT 0,
                'AvgPrice' REAL NOT NULL DEFAULT 0.0,
                FOREIGN KEY('PortfolioId') REFERENCES 'Portfolio'('Id')
            );";
        await command.ExecuteNonQueryAsync();
    }
}