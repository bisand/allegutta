using Microsoft.Data.Sqlite;
using Dapper;
using AlleGutta.Portfolios.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data.Common;

namespace AlleGutta.Repository;
public class PortfolioRepository
{
    private readonly ILogger<PortfolioRepository> _logger;
    private readonly DatabaseOptions _options;

    public PortfolioRepository(IOptions<DatabaseOptions> options, ILogger<PortfolioRepository> logger)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        _logger = logger;
        _options = options.Value;
    }

    public async Task<Portfolio> SavePortfolioAsync(Portfolio portfolio)
    {
        if (portfolio is null) throw new ArgumentNullException(nameof(portfolio), "Portfolio can not be null");
        if (string.IsNullOrWhiteSpace(portfolio.Name)) throw new ArgumentNullException("portfolio.Name", "Portfolio name can not be empty");

        using var connection = new SqliteConnection(_options.ConnectionString);
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();

        try
        {
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
                        Name = @Name COLLATE NOCASE;
                    SELECT Id FROM Portfolio WHERE Name = @Name COLLATE NOCASE;
                ";
                portfolio.Id = await connection.ExecuteScalarAsync<int>(sqlPortfolio, portfolio);
            }

            await SavePortfolioPositionsAsync(portfolio, connection, transaction);
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to save portfolio data.");
            await transaction.RollbackAsync();
        }
        return portfolio;
    }

    private async Task SavePortfolioPositionsAsync(Portfolio portfolio, SqliteConnection connection, DbTransaction? transaction = null)
    {
        if (portfolio == null)
            throw new ArgumentNullException(nameof(portfolio));

        portfolio.Positions ??= new List<PortfolioPosition>();

        if (connection == null)
            throw new ArgumentNullException(nameof(connection));

        var trans = transaction ?? await connection.BeginTransactionAsync();

        try
        {
            var existing = await GetPortfolioPositionsAsync(portfolio.Id).ToListAsync();
            var removed = existing.Except(portfolio.Positions, new PortfolioPositionComparer());
            var added = portfolio.Positions.Except(existing, new PortfolioPositionComparer());
            var updated = portfolio.Positions.Except(removed.Intersect(added), new PortfolioPositionComparer());

            // await connection.ExecuteAsync("DELETE FROM PortfolioPositions WHERE PortfolioId = @Id", portfolio);

            if (removed.Any())
            {
                using var en = removed.GetEnumerator();
                while (en.MoveNext())
                {
                    const string sqlPositions = "DELETE FROM PortfolioPositions WHERE Id = @Id;";
                    en.Current.PortfolioId = portfolio.Id;
                    await connection.ExecuteAsync(sqlPositions, en.Current);
                }
            }

            if (added.Any())
            {
                using var en = added.GetEnumerator();
                while (en.MoveNext())
                {
                    const string sqlPositions = @"
                        INSERT INTO PortfolioPositions 
                        (PortfolioId, Symbol, Shares, AvgPrice, Name, LastPrice, ChangeToday, ChangeTodayPercent, PrevClose, CostValue, CurrentValue, Return, ReturnPercent)
                        VALUES (@PortfolioId, @Symbol, @Shares, @AvgPrice, @Name, @LastPrice, @ChangeToday, @ChangeTodayPercent, @PrevClose, @CostValue, @CurrentValue, @Return, @ReturnPercent);
                        SELECT last_insert_rowid();
                    ";
                    en.Current.PortfolioId = portfolio.Id;
                    en.Current.Id = await connection.ExecuteScalarAsync<int>(sqlPositions, en.Current);
                }
            }

            if (updated.Any())
            {
                using var en = updated.GetEnumerator();
                while (en.MoveNext())
                {
                    const string sqlPositions = @"
                        UPDATE PortfolioPositions SET
                            PortfolioId = @PortfolioId,
                            Symbol = @Symbol,
                            Shares = @Shares,
                            AvgPrice = @AvgPrice,
                            Name = @Name,
                            LastPrice = @LastPrice,
                            ChangeToday = @ChangeToday,
                            ChangeTodayPercent = @ChangeTodayPercent,
                            PrevClose = @PrevClose,
                            CostValue = @CostValue,
                            CurrentValue = @CurrentValue,
                            Return = @Return,
                            ReturnPercent = @ReturnPercent
                        WHERE Id = @Id;
                    ";
                    en.Current.PortfolioId = portfolio.Id;
                    await connection.ExecuteAsync(sqlPositions, en.Current);
                }
            }
            if (transaction != trans)
                await trans.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to save portfolio position data.");
            if (transaction != trans)
                await trans.RollbackAsync();
        }
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
            WHERE p.Name = @portfolioName COLLATE NOCASE;
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
            WHERE p.Id = @portfolioId;
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
            WHERE p.Name = @portfolioName COLLATE NOCASE;
        ";
        await foreach (var item in GetDataAsync<PortfolioPosition>(sql, new[] { new SqliteParameter("@portfolioName", portfolioName) }))
        {
            yield return item;
        }
    }

    public async IAsyncEnumerable<T> GetDataAsync<T>(string sql, IEnumerable<SqliteParameter> parameters)
    {
        using var connection = new SqliteConnection(_options.ConnectionString);
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
