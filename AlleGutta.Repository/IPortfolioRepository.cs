using Microsoft.Data.Sqlite;
using AlleGutta.Portfolios.Models;

namespace AlleGutta.Repository;

public interface IPortfolioRepository
{
    IAsyncEnumerable<T> GetDataAsync<T>(string sql, IEnumerable<SqliteParameter> parameters);
    IAsyncEnumerable<T> GetDataAsync<T>(string sql, IEnumerable<SqliteParameter> parameters);
    Task<Portfolio?> GetPortfolioAsync(string portfolioName);
    Task<Portfolio?> GetPortfolioAsync(string portfolioName);
    IAsyncEnumerable<PortfolioPosition> GetPortfolioPositionsAsync(int portfolioId);
    IAsyncEnumerable<PortfolioPosition> GetPortfolioPositionsAsync(string portfolioName);
    IAsyncEnumerable<PortfolioPosition> GetPortfolioPositionsAsync(int portfolioId);
    IAsyncEnumerable<PortfolioPosition> GetPortfolioPositionsAsync(string portfolioName);
    Task<Portfolio> SavePortfolioAsync(Portfolio portfolio, bool performSummaryUpdate = true, bool performPositionsUpdate = true);
    Task<Portfolio> SavePortfolioAsync(Portfolio portfolio, bool performSummaryUpdate = true, bool performPositionsUpdate = true);
}
