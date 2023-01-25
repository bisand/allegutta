using AlleGutta.Portfolios.Models;
using AlleGutta.Repository;
using AlleGutta.Yahoo;
using AlleGutta.Yahoo.Models;
using Microsoft.AspNetCore.Mvc;

namespace allegutta.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstrumentsController : ControllerBase
{
    private readonly ILogger<InstrumentsController> _logger;
    private readonly YahooApi _yahooApi;

    public InstrumentsController(ILogger<InstrumentsController> logger, YahooApi yahooApi)
    {
        _logger = logger;
        _yahooApi = yahooApi;
    }

    [HttpGet("{symbol}/chart/{range?}/{dataGranularity?}")]
    public async Task<IEnumerable<ChartResult>> GetAsync([FromRoute] string symbol, [FromRoute] string range = "1d", [FromRoute] string dataGranularity = "1m")
    {
        return await _yahooApi.GetChartData(symbol, range, dataGranularity) ?? Array.Empty<ChartResult>();
    }
}
