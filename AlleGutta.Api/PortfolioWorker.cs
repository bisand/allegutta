using AlleGutta.Api.Hubs;
using AlleGutta.Api.Hubs.Clients;
using AlleGutta.Api.Options;
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

    private readonly TimeSpan _runTimeInstrumentHistory;
    private readonly int _requestTimeoutSeconds;
    private DateTime _nextRunInstrumentHistory = DateTime.MinValue;

    private static readonly SemaphoreSlim _mutex = new(1);
    
    private readonly PortfolioProcessor _portfolioProcessor;
    private readonly NordnetWebScraper _webScraper;
    private readonly YahooApi _yahoo;
    private readonly IPortfolioRepository _repository;
    private readonly WorkerOptions _options;
    private readonly IHubContext<PortfolioHub, IPortfolioClient> _portfolioHub;

    public PortfolioWorker(
        ILogger<PortfolioWorker> logger,
        IOptions<WorkerOptions> options,
        IHubContext<PortfolioHub, IPortfolioClient> portfolioHub,
        IPortfolioRepository repository,
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
        _runTimeInstrumentHistory = _options.RunTimeInstrumentHistory;
        _requestTimeoutSeconds = _options.RequestTimeoutSeconds;

        _nextRunInstrumentHistory = DateTime.Now.Date.Add(_runTimeInstrumentHistory);

        _portfolioHub = portfolioHub;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await UpdatePortfolioFromNordnet();
            await UpdatePortfolioWithMarketData();
            await UpdateInstrumentHistory();
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
                _logger.LogInformation("Worker running Nordnet update at: {time}", DateTime.Now);
                var batchData = await _webScraper.GetBatchData();
                var nordnetPortfolio = _portfolioProcessor.GetPortfolioFromBatchData("AlleGutta", batchData);
                await _repository.SavePortfolioAsync(nordnetPortfolio, true, false);
                _logger.LogInformation("Worker done updating Nordnet data at: {time}", DateTime.Now);
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
            await _mutex.WaitAsync();
            try
            {
                if (!_runningUpdateTask && _nextRunMarketData < DateTime.Now)
                {
                    _runningUpdateTask = true;
                    _logger.LogDebug("Worker running Market Data update at: {time}", DateTime.Now);
                    var portfolio = await _repository.GetPortfolioAsync("AlleGutta");
                    if (portfolio?.Positions is not null)
                    {
                        var quotes = await _yahoo.GetQuotes(portfolio.Positions.Select(x => x.Symbol + ".OL"), _requestTimeoutSeconds);
                        if (quotes.Any())
                        {
                            portfolio = _portfolioProcessor.UpdatePortfolioWithMarketData(portfolio, quotes);
                            portfolio.Ath = _options.InitialAth > portfolio.Ath ? _options.InitialAth : portfolio.Ath;
                            await _repository.SavePortfolioAsync(portfolio);
                            await _portfolioHub.Clients.All.PortfolioUpdated(portfolio);
                            _logger.LogInformation("Portfolio updated with market data and saved to database.");
                        }
                        else
                        {
                            _logger.LogWarning("No quotes found for portfolio positions.");
                        }
                    }
                    _logger.LogDebug("Worker done updating Market Data at: {time}", DateTime.Now);
                    _nextRunMarketData = DateTime.Now.Add(_runIntervalMarkedData);
                    _runningUpdateTask = false;
                }
            }
            finally
            {
                _mutex.Release();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving market data from Yahoo!");
            _nextRunMarketData = DateTime.Now.Add(_runIntervalMarkedData);
            _runningUpdateTask = false;
        }
    }

    private async Task UpdateInstrumentHistory()
    {
        try
        {
            if (!_runningUpdateTask && _nextRunInstrumentHistory < DateTime.Now)
            {
                _runningUpdateTask = true;
                _logger.LogDebug("Worker running Instrument History update at: {time}", DateTime.Now);
                var portfolio = await _repository.GetPortfolioAsync("AlleGutta");
                if (portfolio?.Positions is not null)
                {
                    var quotes = await _yahoo.GetQuotes(portfolio.Positions.Select(x => x.Symbol + ".OL"));
                    // await _repository.SavePortfolioAsync(portfolio);
                    // await _portfolioHub.Clients.All.PortfolioUpdated(portfolio);
                }
                _logger.LogDebug("Worker done updating Instrument History at: {time}", DateTime.Now);
                _nextRunInstrumentHistory = _nextRunInstrumentHistory.Date.AddDays(1).Date.Add(_runTimeInstrumentHistory);
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
