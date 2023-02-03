using AlleGutta.Models.Portfolio;
using System.Data.Common;

namespace AlleGutta.Repository;

public interface IPortfolioRepository
{
    string ConnectionString { get; }
    IAsyncEnumerable<T> GetDataAsync<T>(string sql, IEnumerable<DbParameter> parameters);
    Task<Portfolio?> GetPortfolioAsync(string portfolioName);
    IAsyncEnumerable<PortfolioPosition> GetPortfolioPositionsAsync(int portfolioId);
    IAsyncEnumerable<PortfolioPosition> GetPortfolioPositionsAsync(string portfolioName);
    Task<Portfolio> SavePortfolioAsync(Portfolio portfolio, bool performSummaryUpdate = true, bool performPositionsUpdate = true);
}
