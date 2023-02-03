using AlleGutta.Models.Portfolio;

namespace AlleGutta.Api.Hubs.Clients
{
    public interface IPortfolioClient
    {
        Task PortfolioUpdated(Portfolio portfolio);
    }
}