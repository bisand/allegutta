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
        using var transaction = await connection.BeginTransactionAsync();

        if (await GetPortfolioAsync(portfolio.Name) is null)
        {
            const string sqlPortfolio = @"
                INSERT INTO Portfolio
                (Name, Cash, Ath, Equity, CostValue, MarketValue, MarketValuePrev, MarketValueMax, MarketValueMin, ChangeTodayTotal, ChangeTodayPercent, ChangeTotal, ChangeTotalPercent)
                VALUES (@Name, @Cash, @Ath, @Equity, @CostValue, @MarketValue, @MarketValuePrev, @MarketValueMax, @MarketValueMin, @ChangeTodayTotal, @ChangeTodayPercent, @ChangeTotal, @ChangeTotalPercent);
                SELECT last_insert_rowid();
            ";
            portfolio.Id = await connection.ExecuteScalarAsync<int>(sqlPortfolio, portfolio);
        }
        else
        {
            const string sqlPortfolio = @"
                UPDATE Portfolio SET
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
                    Name = @Name;
                SELECT Id FROM Portfolio WHERE Name = @Name;
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
                        (PortfolioId, Symbol, Shares, AvgPrice, Name, LastPrice, ChangeToday, ChangeTodayPercent, PrevClose, CostValue, CurrentValue, Return, ReturnPercent)
                        VALUES (@PortfolioId, @Symbol, @Shares, @AvgPrice, @Name, @LastPrice, @ChangeToday, @ChangeTodayPercent, @PrevClose, @CostValue, @CurrentValue, @Return, @ReturnPercent);
                        SELECT last_insert_rowid();
                ";
                pos.PortfolioId = portfolio.Id;
                pos.Id = await connection.ExecuteScalarAsync<int>(sqlPositions, pos);
            }
        }
        await transaction.CommitAsync();
        return portfolio;
    }

    public async Task<Portfolio?> GetPortfolioAsync(string portfolioName)
    {
        if (string.IsNullOrWhiteSpace(portfolioName)) throw new ArgumentNullException(nameof(portfolioName), "Portfolio name can not be empty");

        const string sql = @"
            SELECT
                p.Id,
                p.Name,
                CAST(p.Cash as DOUBLE) as Cash,
                CAST(p.Ath as DOUBLE) as Ath,
                CAST(p.Equity as DOUBLE) as Equity,
                CAST(p.CostValue as DOUBLE) as CostValue,
                CAST(p.MarketValue as DOUBLE) as MarketValue,
                CAST(p.MarketValuePrev as DOUBLE) as MarketValuePrev,
                CAST(p.MarketValueMax as DOUBLE) as MarketValueMax,
                CAST(p.MarketValueMin as DOUBLE) as MarketValueMin,
                CAST(p.ChangeTodayTotal as DOUBLE) as ChangeTodayTotal,
                CAST(p.ChangeTodayPercent as DOUBLE) as ChangeTodayPercent,
                CAST(p.ChangeTotal as DOUBLE) as ChangeTotal,
                CAST(p.ChangeTotalPercent as DOUBLE) as ChangeTotalPercent
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
            SELECT
                pp.Id,
                pp.PortfolioId,
                pp.Symbol,
                pp.Shares,
                CAST(pp.AvgPrice as DOUBLE) as AvgPrice,
                pp.Name,
                CAST(pp.LastPrice as DOUBLE) as LastPrice,
                CAST(pp.ChangeToday as DOUBLE) as ChangeToday,
                CAST(pp.ChangeTodayPercent as DOUBLE) as ChangeTodayPercent,
                CAST(pp.PrevClose as DOUBLE) as PrevClose,
                CAST(pp.CostValue as DOUBLE) as CostValue,
                CAST(pp.CurrentValue as DOUBLE) as CurrentValue,
                CAST(pp.Return as DOUBLE) as Return,
                CAST(pp.ReturnPercent as DOUBLE) as ReturnPercent
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
            SELECT
                pp.Id,
                pp.PortfolioId,
                pp.Symbol,
                pp.Shares,
                CAST(pp.AvgPrice as DOUBLE) as AvgPrice,
                pp.Name,
                CAST(pp.LastPrice as DOUBLE) as LastPrice,
                CAST(pp.ChangeToday as DOUBLE) as ChangeToday,
                CAST(pp.ChangeTodayPercent as DOUBLE) as ChangeTodayPercent,
                CAST(pp.PrevClose as DOUBLE) as PrevClose,
                CAST(pp.CostValue as DOUBLE) as CostValue,
                CAST(pp.CurrentValue as DOUBLE) as CurrentValue,
                CAST(pp.Return as DOUBLE) as Return,
                CAST(pp.ReturnPercent as DOUBLE) as ReturnPercent
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