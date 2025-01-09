using AlleGutta.Nordnet.Models;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using System.Linq;
using System.Text.Json;

namespace AlleGutta.Nordnet;

public class NordnetWebScraper
{
    private static readonly NordnetBatchData BatchData = new();
    private readonly NordNetConfig _config;
    private readonly ILogger<NordnetWebScraper> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private int _accountId = 0;
    private int _dataCollected = 0;
    private SemaphoreSlim _accountIdSemaphore = new(1, 1);
    private SemaphoreSlim _portfolioSemaphore = new(1, 1);

    public NordnetWebScraper(NordNetConfig config, ILogger<NordnetWebScraper> logger)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config), "Configuration cannot be empty!");
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    private static void CheckCancellation(CancellationToken cancellationToken, string message = "Operation cancelled.")
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
        }
        catch (OperationCanceledException)
        {
            throw new OperationCanceledException(message);
        }
    }

    /// <summary>
    /// Return Batch data from Nordnet API. Is used to retrive portfolio specific data.
    /// </summary>
    /// <param name="forceRun">Force execution and override refresh interval.</param>
    /// <param name="refreshIntervalMinutes">Refresh interval in minutes. If time is within interval, cched data are returned. Otherwise a request is being made to Nordnet.</param>
    /// <param name="headless">Indicate if Chrome should run in headless mode. Default true.</param>
    /// <returns>NordnetBatchData</returns>
    public async Task<NordnetBatchData> GetBatchData(bool forceRun = false, int refreshIntervalMinutes = 60, bool headless = true, CancellationToken cancellationToken = default)
    {
        bool isCacheValid = BatchData.CacheUpdated is not null && BatchData.CacheUpdated.Value.AddMinutes(refreshIntervalMinutes) > DateTime.Now;
        if (!forceRun && BatchData.CacheUpdated != null && isCacheValid)
        {
            return BatchData;
        }

        var options = new LaunchOptions
        {
            Headless = headless,
            DefaultViewport = { Width = 1024, Height = 768 },
            Args = ["--disable-dev-shm-usage", "--no-sandbox"],
            // ExecutablePath = "/usr/bin/chromium"
        };

        IBrowser? browser = null;
        try
        {
            CheckCancellation(cancellationToken);
            browser = await Puppeteer.LaunchAsync(options).ConfigureAwait(false);
            CheckCancellation(cancellationToken);
            using var page = await browser.NewPageAsync().ConfigureAwait(false);
            CheckCancellation(cancellationToken);
            await Login(page);
            page.Response += GetAccountIdEventHandler;
            CheckCancellation(cancellationToken);
            _ = await page.GoToAsync($"https://www.nordnet.no/overview/details", WaitUntilNavigation.Networkidle2);
            CheckCancellation(cancellationToken);
            await WaitForConditionAsync(() => _accountId > 0, 100, $"Timed out while processing data from: https://www.nordnet.no/overview/details", cancellationToken);
            CheckCancellation(cancellationToken);
            page.Response -= GetAccountIdEventHandler;

            page.Response += GetPortfolioEventHandler;
            _ = await page.GoToAsync($"https://www.nordnet.no/overview/details/{_accountId}", WaitUntilNavigation.Networkidle2);
            CheckCancellation(cancellationToken);
            await WaitForConditionAsync(() => _dataCollected >= 2, 100, $"Timed out while processing data from: https://www.nordnet.no/overview/details/{_accountId}", cancellationToken);
            page.Response -= GetPortfolioEventHandler;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while processing Nordnet data.");
        }
        finally
        {
            if (browser != null)
            {
                await browser.CloseAsync();
                await browser.DisposeAsync();
            }
        }
        return BatchData;
    }

    // Method to run GetBatchData with a cancellation token
    public async Task<NordnetBatchData> GetBatchData(CancellationToken cancellationToken)
    {
        return await GetBatchData(false, 60, true, cancellationToken);
    }

    private async void GetAccountIdEventHandler(object? sender, ResponseCreatedEventArgs responseEvent)
    {
        var response = responseEvent.Response;

        if (!response.Ok)
        {
            _logger.LogWarning($"Response NOT OK: {response.Status}: {response.StatusText}");
            return;
        }

        await _accountIdSemaphore.WaitAsync();

        try
        {
            var request = response.Request;
            var headers = response.Headers;

            var url = request.Url;
            var postDataText = (request.PostData ?? string.Empty).ToString();
            var isAPI = url != null && url.Contains("/api/2/accounts");
            var isGET = request.Method == HttpMethod.Get;
            var isJson = headers.TryGetValue("content-type", out string? contentType) && (contentType?.Contains("application/json") == true);

            if (isAPI && isGET && isJson)
            {
                if (url != null && url.EndsWith("/api/2/accounts"))
                {
                    try
                    {
                        // TODO: Fix this to handle multiple accounts or apply better error handling
                        var txt = await response.TextAsync();
                        var accounts = JsonSerializer.Deserialize<NordnetAccount[]>(txt, _jsonSerializerOptions);
                        if (accounts != null)
                        {
                            foreach (var account in accounts)
                            {
                                if (account?.Accno == _config.AccountNo)
                                {
                                    _logger.LogInformation($"Found account: {account.Accno} - {account.Accid}");
                                    _accountId = account.Accid;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "An error ocurred while handling GET JSON data.");
                    }
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while processing Nordnet data.");
        }
        finally
        {
            _accountIdSemaphore.Release();
        }
    }

    private async void GetPortfolioEventHandler(object? sender, ResponseCreatedEventArgs responseEvent)
    {
        var response = responseEvent.Response;

        if (!response.Ok)
        {
            _logger.LogWarning($"Response NOT OK: {response.Status}: {response.StatusText}");
            return;
        }

        await _portfolioSemaphore.WaitAsync();

        try
        {
            var request = response.Request;
            var headers = response.Headers;

            var url = request.Url;
            var postDataText = (request.PostData ?? string.Empty).ToString();
            var isAPI = url != null && (url.Contains("/api/2/batch") || url.Contains("/api/2/accounts"));
            var isGET = request.Method == HttpMethod.Get;
            var isPOST = request.Method == HttpMethod.Post;
            var isJson = headers.TryGetValue("content-type", out string? contentType) && (contentType?.Contains("application/json") == true);

            if (isAPI && isPOST && isJson)
            {
                try
                {
                    var strBatch = JsonSerializer.Deserialize<NordnetRequestStringBatch>(postDataText ?? string.Empty, _jsonSerializerOptions);
                    var postData = JsonSerializer.Deserialize<NordnetRequest[]>(strBatch?.Batch ?? string.Empty, _jsonSerializerOptions);

                    if (postData?.GetType().IsArray == true)
                    {
                        for (var i = 0; i < postData.Length; i++)
                        {
                            if (postData[i].RelativeUrl.Contains($"accounts/{_accountId}/positions"))
                            {
                                _logger.LogInformation($"Found positions: {postData[i].RelativeUrl}");
                                var txt = await response.TextAsync();
                                var json = JsonSerializer.Deserialize<NordnetJsonContent<NordnetPosition[]>[]>(txt, _jsonSerializerOptions);
                                _dataCollected = CollectPositions(json?[i].Body, _dataCollected);
                            }
                            else if (postData[i].RelativeUrl.Contains($"accounts/{_accountId}/info"))
                            {
                                _logger.LogInformation($"Found account info: {postData[i].RelativeUrl}");
                                var txt = await response.TextAsync();
                                var json = JsonSerializer.Deserialize<NordnetJsonContent<NordnetAccountInfo[]>[]>(txt, _jsonSerializerOptions);
                                _dataCollected = CollectAccountInfo(json?[i].Body?[0], _dataCollected);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error ocurred while handling POST batch data.");
                }
            }
            else if (isAPI && isJson)
            {
                try
                {
                    if (url?.Contains($"accounts/{_accountId}/positions") == true)
                    {
                        var txt = await response.TextAsync();
                        var json = JsonSerializer.Deserialize<NordnetPosition[]>(txt, _jsonSerializerOptions);
                        _dataCollected = CollectPositions(json, _dataCollected);
                    }
                    else if (url?.Contains($"accounts/{_accountId}/info") == true)
                    {
                        var txt = await response.TextAsync();
                        var json = JsonSerializer.Deserialize<NordnetAccountInfo[]>(txt, _jsonSerializerOptions);
                        _dataCollected = CollectAccountInfo(json?[0], _dataCollected);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error ocurred while handling JSON batch data.");
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while processing Nordnet data.");
        }
        finally
        {
            _portfolioSemaphore.Release();
        }
    }

    /// <summary>
    /// Handles the Nordnet login process.
    /// </summary>
    /// <param name="page">Puppeteer page</param>
    /// <returns></returns>
    private async Task Login(IPage page)
    {
        await page.GoToAsync(_config.Url);
        await page.ClickAsync("button#cookie-accept-all-secondary");
        await page.WaitForSelectorAsync("xpath///button[contains(., 'innloggingsmetode')]", new() { Timeout = 10000 });
        var button1 = await page.QuerySelectorAsync("xpath///button[contains(., 'innloggingsmetode')]");
        if (button1 != null)
        {
            await button1.ClickAsync();
        }
        await page.WaitForSelectorAsync("xpath///button[contains(., 'brukernavn og passord')]", new() { Timeout = 10000 });
        var button2 = await page.QuerySelectorAsync("xpath///button[contains(., 'brukernavn og passord')]");
        if (button2 != null)
        {
            await button2.ClickAsync();
        }

        await page.WaitForSelectorAsync("input[name='username']", new() { Timeout = 10000 });
        await page.TypeAsync("input[name='username']", _config.Username);
        await page.TypeAsync("input[name='password']", _config.Password);

        Task.WaitAll([
                page.ClickAsync("button[type='submit']"),
                page.WaitForNavigationAsync()
            ]);
    }

    public async Task WaitForConditionAsync(Func<bool> condition, int intervalMs = 100, string timeoutMessage = "Timeout!", CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Waiting for condition to be true...");
        while (!condition())
        {
            CheckCancellation(cancellationToken, timeoutMessage);
            await Task.Delay(intervalMs, cancellationToken);
        }
        _logger.LogDebug("Condition is now true.");
    }

    private static int CollectAccountInfo(NordnetAccountInfo? json, int dataCollected)
    {
        BatchData.AccountInfo = json;
        BatchData.CacheUpdated = DateTime.Now;
        dataCollected++;
        return dataCollected;
    }

    private static int CollectPositions(NordnetPosition[]? json, int dataCollected)
    {
        BatchData.Positions = json;
        BatchData.CacheUpdated = DateTime.Now;
        dataCollected++;
        return dataCollected;
    }
}