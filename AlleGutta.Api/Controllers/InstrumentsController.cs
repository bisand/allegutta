using AlleGutta.Portfolios.Models;
using AlleGutta.Repository;
using AlleGutta.Yahoo;
using AlleGutta.Yahoo.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public async Task<Results<BadRequest, Ok<IEnumerable<ChartResult>>>> GetChartAsync([FromRoute] string symbol, [FromRoute] string range = "1d", [FromRoute] string dataGranularity = "1m")
    {
        if (symbol is null || symbol.Length < 1)
        {
            return TypedResults.BadRequest();
        }
        return TypedResults.Ok(await _yahooApi.GetChartData(symbol, range, dataGranularity) ?? Array.Empty<ChartResult>());
    }

    [HttpGet("{symbol}")]
    public async Task<Results<NotFound, Ok<OptionQuote>>> GetInstrumentAsync([FromRoute] string symbol)
    {
        var result = await _yahooApi.GetInstrumentData(symbol);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }
}
