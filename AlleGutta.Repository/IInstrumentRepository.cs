using AlleGutta.Portfolios.Models;
using AlleGutta.Models.Yahoo;
using System.Data.Common;

namespace AlleGutta.Repository;

public interface IInstrumentRepository
{
    string ConnectionString { get; }
    IAsyncEnumerable<T> GetDataAsync<T>(string sql, IEnumerable<DbParameter> parameters);
    IAsyncEnumerable<OptionQuote> GetInstrumentInfoAsync(IEnumerable<string> symbols);
    IAsyncEnumerable<PortfolioPosition> GetPortfolioPositionsAsync(int portfolioId);
    IAsyncEnumerable<PortfolioPosition> GetPortfolioPositionsAsync(string portfolioName);
    Task<Portfolio> SavePortfolioAsync(Portfolio portfolio, bool performSummaryUpdate = true, bool performPositionsUpdate = true);
}
