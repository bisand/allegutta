using AlleGutta.Api.Hubs;
using AlleGutta.Api.Hubs.Clients;
using AlleGutta.Nordnet;
using AlleGutta.Portfolios;
using AlleGutta.Repository;
using AlleGutta.Yahoo;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace AlleGutta.Api;

public sealed class PortfolioWorker : BackgroundService
{
    private readonly TimeSpan _executionInterval;
    private readonly ILogger<PortfolioWorker> _logger;
    private bool _runningUpdateTask;

    private readonly TimeSpan _runIntervalNordnet;
    private DateTime _nextRunNordnet = DateTime.MinValue;

    private readonly TimeSpan _runIntervalMarkedData;
    private DateTime _nextRunMarketData = DateTime.MinValue;

    private readonly PortfolioProcessor _portfolioProcessor;
    private readonly NordnetWebScraper _webScraper;
    private readonly YahooApi _yahoo;
    private readonly PortfolioRepository _repository;
    private readonly WorkerOptions _options;
    private readonly IHubContext<PortfolioHub, IPortfolioClient> _portfolioHub;

    public PortfolioWorker(
        ILogger<PortfolioWorker> logger,
        IOptions<WorkerOptions> options,
        IHubContext<PortfolioHub, IPortfolioClient> portfolioHub,
        PortfolioRepository repository,
        PortfolioProcessor portfolioProcessor,
        NordnetWebScraper webScraper,
        YahooApi yahoo)
    {
        _logger = logger;
        _portfolioProcessor = portfolioProcessor ?? throw new ArgumentNullException(nameof(portfolioProcessor));
        _webScraper = webScraper ?? throw new ArgumentNullException(nameof(webScraper));
        _yahoo = yahoo ?? throw new ArgumentNullException(nameof(yahoo));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _options = options.Value;
        _executionInterval = _options.ExecutionInterval;
        _runIntervalNordnet = _options.RunIntervalNordnet;
        _runIntervalMarkedData = _options.RunIntervalMarkedData;
        _portfolioHub = portfolioHub;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await UpdatePortfolioFromNordnet();
            await UpdatePortfolioWithMarketData();
            await Task.Delay(_executionInterval, stoppingToken);
        }
    }

    private async Task UpdatePortfolioFromNordnet()
    {
        try
        {
            if (!_runningUpdateTask && _nextRunNordnet < DateTime.Now)
            {
                _runningUpdateTask = true;
                _logger.LogDebug("Worker running Nordnet update at: {time}", DateTimeOffset.Now);
                var batchData = await _webScraper.GetBatchData();
                var nordnetPortfolio = _portfolioProcessor.GetPortfolioFromBatchData("AlleGutta", batchData);
                await _repository.SavePortfolioAsync(nordnetPortfolio, false);
                _logger.LogDebug("Worker done updating Nordnet data at: {time}", DateTimeOffset.Now);
                _nextRunNordnet = DateTime.Now.Add(_runIntervalNordnet);
                _runningUpdateTask = false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving data from Nordnet!");
            _nextRunNordnet = DateTime.Now.Add(_runIntervalNordnet);
            _runningUpdateTask = false;
        }
    }

    private async Task UpdatePortfolioWithMarketData()
    {
        try
        {
            if (!_runningUpdateTask && _nextRunMarketData < DateTime.Now)
            {
                _runningUpdateTask = true;
                _logger.LogDebug("Worker running Market Data update at: {time}", DateTimeOffset.Now);
                var portfolio = await _repository.GetPortfolioAsync("AlleGutta");
                if (portfolio?.Positions is not null)
                {
                    var quotes = await _yahoo.GetQuotes(portfolio.Positions.Select(x => x.Symbol + ".OL"));
                    portfolio = _portfolioProcessor.UpdatePortfolioWithMarketData(portfolio, quotes);
                    await _repository.SavePortfolioAsync(portfolio);
                    await _portfolioHub.Clients.All.PortfolioUpdated(portfolio);
                }
                _logger.LogDebug("Worker done updating Market Data at: {time}", DateTimeOffset.Now);
                _nextRunMarketData = DateTime.Now.Add(_runIntervalMarkedData);
                _runningUpdateTask = false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving market data from Yahoo!");
            _nextRunMarketData = DateTime.Now.Add(_runIntervalMarkedData);
            _runningUpdateTask = false;
        }
    }
}
