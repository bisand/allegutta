using AlleGutta.Portfolios.Models;
using AlleGutta.Repository;
using Microsoft.AspNetCore.Mvc;

namespace allegutta.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PortfolioController : ControllerBase
{
    private readonly ILogger<PortfolioController> _logger;
    private readonly PortfolioRepositorySQLite _portfolioRepository;

    public PortfolioController(ILogger<PortfolioController> logger, PortfolioRepositorySQLite portfolioRepository)
    {
        _logger = logger;
        _portfolioRepository = portfolioRepository;
    }

    [HttpGet("{portfolioName}")]
    public async Task<Portfolio?> GetAsync([FromRoute]string portfolioName)
    {
        return await _portfolioRepository.GetPortfolioAsync(portfolioName) ?? new Portfolio();
    }
}
