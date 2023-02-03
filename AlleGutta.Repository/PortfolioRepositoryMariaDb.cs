using System.Data.Common;
using AlleGutta.Models.Portfolio;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace AlleGutta.Repository;
public class PortfolioRepositoryMariaDb : BaseRepositoryMariaDb, IPortfolioRepository
{
    public PortfolioRepositoryMariaDb(IOptions<DatabaseOptionsMariaDb> options, ILogger<PortfolioRepositoryMariaDb> logger) : base(options, logger)
    {
    }

    public async Task<Portfolio> SavePortfolioAsync(Portfolio portfolio, bool performSummaryUpdate = true, bool performPositionsUpdate = true)
    {
        if (portfolio is null) throw new ArgumentNullException(nameof(portfolio), "Portfolio can not be null");
        if (string.IsNullOrWhiteSpace(portfolio.Name)) throw new ArgumentNullException("portfolio.Name", "Portfolio name can not be empty");

        using var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var existingPortfolio = await GetPortfolioAsync(portfolio.Name);
            if (existingPortfolio is null)
            {
                const string sqlPortfolio = @"
                    INSERT INTO Portfolio
                    (Name, Cash, Ath, Equity, CostValue, MarketValue, MarketValuePrev, MarketValueMax, MarketValueMin, ChangeTodayTotal, ChangeTodayPercent, ChangeTotal, ChangeTotalPercent)
                    VALUES (@Name, @Cash, @Ath, @Equity, @CostValue, @MarketValue, @MarketValuePrev, @MarketValueMax, @MarketValueMin, @ChangeTodayTotal, @ChangeTodayPercent, @ChangeTotal, @ChangeTotalPercent);
                    SELECT LAST_INSERT_ID();
                ";
                portfolio.Id = await connection.ExecuteScalarAsync<int>(sqlPortfolio, portfolio, transaction);
            }
            else if (performSummaryUpdate)
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
                portfolio.Id = await connection.ExecuteScalarAsync<int>(sqlPortfolio, portfolio, transaction);
            }
            else if (existingPortfolio != null)
            {
                portfolio.Id = existingPortfolio.Id;
            }

            await SavePortfolioPositionsAsync(portfolio, connection, transaction, performPositionsUpdate);
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to save portfolio data.");
            await transaction.RollbackAsync();
        }
        return portfolio;
    }

    private async Task SavePortfolioPositionsAsync(Portfolio portfolio, MySqlConnection connection, DbTransaction? transaction = null, bool performUpdate = true)
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
            var incoming = portfolio.Positions;
            var comparer = new PortfolioPositionComparer();
            var removed = existing.Except(incoming, comparer);
            var added = incoming.Except(existing, comparer);
            var updated = incoming.Intersect(existing, comparer);

            // await connection.ExecuteAsync("DELETE FROM PortfolioPositions WHERE PortfolioId = @Id", portfolio);

            if (removed.Any())
            {
                using var en = removed.GetEnumerator();
                while (en.MoveNext())
                {
                    const string sqlPositions = "DELETE FROM PortfolioPositions WHERE Id = @Id;";
                    en.Current.PortfolioId = portfolio.Id;
                    await connection.ExecuteAsync(sqlPositions, en.Current, trans);
                }
            }

            if (added.Any())
            {
                using var en = added.GetEnumerator();
                while (en.MoveNext())
                {
                    const string sqlPositions = @"
                        INSERT INTO PortfolioPositions 
                        (PortfolioId, Symbol, Shares, AvgPrice, Name, LastPrice, ChangeToday, ChangeTodayPercent, PrevClose, CostValue, CurrentValue, ReturnValue, ReturnPercent, DateAdded, DateModified)
                        VALUES (@PortfolioId, @Symbol, @Shares, @AvgPrice, @Name, @LastPrice, @ChangeToday, @ChangeTodayPercent, @PrevClose, @CostValue, @CurrentValue, @ReturnValue, @ReturnPercent, NOW(), NOW());
                        SELECT LAST_INSERT_ID();
                    ";
                    en.Current.PortfolioId = portfolio.Id;
                    en.Current.Id = await connection.ExecuteScalarAsync<int>(sqlPositions, en.Current, trans);
                }
            }

            if (performUpdate && updated.Any())
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
                            ReturnValue = @ReturnValue,
                            ReturnPercent = @ReturnPercent,
                            DateModified = NOW()
                        WHERE Id = @Id;
                    ";
                    en.Current.PortfolioId = portfolio.Id;
                    await connection.ExecuteAsync(sqlPositions, en.Current, trans);
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
                p.Cash,
                p.Ath,
                p.Equity,
                p.CostValue,
                p.MarketValue,
                p.MarketValuePrev,
                p.MarketValueMax,
                p.MarketValueMin,
                p.ChangeTodayTotal,
                p.ChangeTodayPercent,
                p.ChangeTotal,
                p.ChangeTotalPercent
            FROM Portfolio p
            WHERE p.Name = @portfolioName;
        ";
        var portfolio = await GetDataAsync<Portfolio>(sql, new[] { new MySqlParameter("@portfolioName", portfolioName) }).FirstOrDefaultAsync();
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
                pp.AvgPrice,
                pp.Name,
                pp.LastPrice,
                pp.ChangeToday,
                pp.ChangeTodayPercent,
                pp.PrevClose,
                pp.CostValue,
                pp.CurrentValue,
                pp.ReturnValue,
                pp.ReturnPercent
            FROM PortfolioPositions pp
            JOIN Portfolio p ON p.Id = pp.PortfolioId
            WHERE p.Id = @portfolioId;
        ";
        await foreach (var item in GetDataAsync<PortfolioPosition>(sql, new[] { new MySqlParameter("@portfolioId", portfolioId) }))
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
                pp.AvgPrice,
                pp.Name,
                pp.LastPrice,
                pp.ChangeToday,
                pp.ChangeTodayPercent,
                pp.PrevClose,
                pp.CostValue,
                pp.CurrentValue,
                pp.ReturnValue,
                pp.ReturnPercent
            FROM PortfolioPositions pp
            JOIN Portfolio p ON p.Id = pp.PortfolioId
            WHERE p.Name = @portfolioName;
        ";
        await foreach (var item in GetDataAsync<PortfolioPosition>(sql, new[] { new MySqlParameter("@portfolioName", portfolioName) }))
        {
            yield return item;
        }
    }

}