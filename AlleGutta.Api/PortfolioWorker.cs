using AlleGutta.Nordnet;
using AlleGutta.Portfolios;
using AlleGutta.Repository;
using AlleGutta.Yahoo;

namespace App.WorkerService;

public sealed class PortfolioWorker : BackgroundService
{
    private readonly TimeSpan _executionInterval = new(0, 0, 1);
    private readonly ILogger<PortfolioWorker> _logger;
    private bool _runningUpdateTask;

    private readonly TimeSpan _runIntervalNordnet = new(5, 0, 0);
    private DateTime _nextRunNordnet = DateTime.MinValue;

    private readonly TimeSpan _runIntervalMarkedData = new(0, 0, 10);
    private DateTime _nextRunMarketData = DateTime.MinValue;

    private readonly PortfolioProcessor _portfolioProcessor;
    private readonly NordnetWebScraper _webScraper;
    private readonly Yahoo _yahoo;
    private readonly PortfolioRepository _repository;

    public PortfolioWorker(
        ILogger<PortfolioWorker> logger,
        PortfolioRepository repository,
        PortfolioProcessor portfolioProcessor,
        NordnetWebScraper webScraper,
        Yahoo yahoo)
    {
        _logger = logger;
        _portfolioProcessor = portfolioProcessor ?? throw new ArgumentNullException(nameof(portfolioProcessor));
        _webScraper = webScraper ?? throw new ArgumentNullException(nameof(webScraper));
        _yahoo = yahoo ?? throw new ArgumentNullException(nameof(yahoo));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
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
        if (!_runningUpdateTask && _nextRunNordnet < DateTime.Now)
        {
            _runningUpdateTask = true;
            _logger.LogInformation("Worker running Nordnet update at: {time}", DateTimeOffset.Now);
            var portfolio = await _repository.GetPortfolioAsync("AlleGutta");
            _nextRunNordnet = DateTime.Now.Add(_runIntervalNordnet);
            _runningUpdateTask = false;
        }
    }

    private async Task UpdatePortfolioWithMarketData()
    {
        if (!_runningUpdateTask && _nextRunMarketData < DateTime.Now)
        {
            _runningUpdateTask = true;
            _nextRunMarketData = DateTime.Now.Add(_runIntervalMarkedData);
            var portfolio = await _repository.GetPortfolioAsync("AlleGutta");
            _logger.LogInformation("Worker running Market Data update at: {time}", DateTimeOffset.Now);
            _runningUpdateTask = false;
        }
    }
}
