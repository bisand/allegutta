using AlleGutta.Portfolios.Models;

namespace AlleGutta.Api.Hubs.Clients
{
    public interface IPortfolioClient
    {
        Task PortfolioUpdated(Portfolio portfolio);
    }
}