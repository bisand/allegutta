using Microsoft.Data.Sqlite;
using Dapper;
using AlleGutta.Models.Nordnet;
using AlleGutta.Models;
using System.Linq;

namespace AlleGutta.Repository;
public class PortfolioData
{
    private readonly string _connectionString;

    public PortfolioData(string ConnectionString)
    {
        _connectionString = ConnectionString;
    }

    public async Task<Portfolio> SavePortfolioAsync(Portfolio portfolio)
    {
        if (portfolio is null) throw new ArgumentNullException(nameof(portfolio), "Portfolio can not be null");
        if (string.IsNullOrWhiteSpace(portfolio.Name)) throw new ArgumentNullException("portfolio.Name", "Portfolio name can not be empty");

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        if (GetPortfolioAsync(portfolio.Name) is null)
        {
            const string sqlPortfolio = @"
            INSERT INTO Portfolio 
            OUTPUT INSERTED.Id
            VALUES (@Name, @Cash, @Ath, @Equity, @CostValue, @MarketValue, @MarketValuePrev, @MarketValueMax, @MarketValueMin, @ChangeTodayTotal, @ChangeTodayPercent, @ChangeTotal, @ChangeTotalPercent)
        ";
            portfolio.Id = await connection.ExecuteScalarAsync<int>(sqlPortfolio, portfolio);
        }
        else
        {
            const string sqlPortfolio = @"
                UPDATE Portfolio 
                OUTPUT UPDATED.Id
                    Name = @Name,
                    Cash = @Cash,
                    Ath = @Ath,
                    Equity = @Equity,
                    CostValue = @CostValue,
                    MarketValue = @MarketValue,
                    MarketValuePrev = @MarketValuePrev,
                    MarketValueMax = @MarketValueMax,
                    MarketValueMin = @MarketValueMin,
                    ChangeTodayTotal = @ChangeTodayTotal,
                    ChangeTodayPercent = @ChangeTodayPercent,
                    ChangeTotal = @ChangeTotal,
                    ChangeTotalPercent = @ChangeTotalPercent
                WHERE
                    Name = @Name
            ";
            portfolio.Id = await connection.ExecuteScalarAsync<int>(sqlPortfolio, portfolio);
        }

        await connection.ExecuteAsync("DELETE FROM PortfolioPositions WHERE PortfolioId = @Id", portfolio);

        if (portfolio.Positions != null)
        {
            foreach (var pos in portfolio.Positions)
            {
                const string sqlPositions = @"
                        INSERT INTO PortfolioPositions 
                        OUTPUT INSERTED.Id
                        VALUES (@Symbol, @Shares, @AvgPrice, @Name, @LastPrice, @ChangeToday, @ChangeTodayPercent, @PrevClose, @CostValue, @CurrentValue, @Return, @ReturnPercent)";
                pos.Id = await connection.ExecuteScalarAsync<int>(sqlPositions, pos);
                pos.PortfolioId = portfolio.Id;
            }
        }

        return portfolio;
    }

    public async Task<Portfolio?> GetPortfolioAsync(string portfolioName)
    {
        if (string.IsNullOrWhiteSpace(portfolioName)) throw new ArgumentNullException(nameof(portfolioName), "Portfolio name can not be empty");

        const string sql = @"
            SELECT p.Id, p.Name, p.Cash, p.Ath, p.Equity, p.CostValue, p.MarketValue, p.MarketValuePrev, p.MarketValueMax, p.MarketValueMin, p.ChangeTodayTotal, p.ChangeTodayPercent, p.ChangeTotal, p.ChangeTotalPercent
            FROM Portfolio p
            WHERE p.Name = @portfolioName
        ";
        var portfolio = await GetDataAsync<Portfolio>(sql, new[] { new SqliteParameter("@portfolioName", portfolioName) }).FirstOrDefaultAsync();
        if (portfolio != null) portfolio.Positions = await GetPortfolioPositionsAsync(portfolio.Id).ToArrayAsync();
        return portfolio;
    }

    public async IAsyncEnumerable<PortfolioPosition> GetPortfolioPositionsAsync(int portfolioId)
    {
        const string sql = @"
            SELECT pp.Id, pp.PortfolioId, pp.Symbol, pp.Shares, pp.AvgPrice, pp.Name, pp.LastPrice, pp.ChangeToday, pp.ChangeTodayPercent, pp.PrevClose, pp.CostValue, pp.CurrentValue, pp.Return, pp.ReturnPercent 
            FROM PortfolioPositions pp
            JOIN Portfolio p ON p.Id = pp.PortfolioId
            WHERE p.Id = @portfolioId
        ";
        await foreach (var item in GetDataAsync<PortfolioPosition>(sql, new[] { new SqliteParameter("@portfolioId", portfolioId) }))
        {
            yield return item;
        }
    }

    public async IAsyncEnumerable<PortfolioPosition> GetPortfolioPositionsAsync(string portfolioName)
    {
        const string sql = @"
            SELECT pp.Id, pp.PortfolioId, pp.Symbol, pp.Shares, pp.AvgPrice, pp.Name, pp.LastPrice, pp.ChangeToday, pp.ChangeTodayPercent, pp.PrevClose, pp.CostValue, pp.CurrentValue, pp.Return, pp.ReturnPercent 
            FROM PortfolioPositions pp
            JOIN Portfolio p ON p.Id = pp.PortfolioId
            WHERE p.Name = @portfolioName
        ";
        await foreach (var item in GetDataAsync<PortfolioPosition>(sql, new[] { new SqliteParameter("@portfolioName", portfolioName) }))
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
}