using AlleGutta.Api.Hubs.Clients;
using AlleGutta.Models.Portfolio;
using Microsoft.AspNetCore.SignalR;

namespace AlleGutta.Api.Hubs;

public class PortfolioHub : Hub<IPortfolioClient>
{
    public async Task PublishPortfolio(Portfolio portfolio)
    {
        await Clients.All.PortfolioUpdated(portfolio);
    }
}