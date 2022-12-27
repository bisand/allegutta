using AlleGutta.Api.Hubs;
using AlleGutta.Api.Hubs.Clients;
using AlleGutta.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AlleGutta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InfoController : ControllerBase
{
    private readonly ILogger<InfoController> _logger;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IHubContext<PortfolioHub, IPortfolioClient> _portfolioHub;

    public InfoController(IPortfolioRepository portfolioRepository, IHubContext<PortfolioHub, IPortfolioClient> portfolioHub, ILogger<InfoController> logger)
    {
        _portfolioRepository = portfolioRepository ?? throw new ArgumentNullException(nameof(portfolioRepository));
        _portfolioHub = portfolioHub ?? throw new ArgumentNullException(nameof(portfolioHub));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<object> GetUsersAsync()
    {
        return null;
    }
}
