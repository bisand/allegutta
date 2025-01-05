using AlleGutta.Nordnet.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PuppeteerSharp;
using System.Linq;

namespace AlleGutta.Nordnet;

public class NordnetWebScraper
{
    private static readonly NordnetBatchData BatchData = new();
    private readonly NordNetConfig _config;
    private readonly ILogger<NordnetWebScraper> _logger;
    private int _accountId = 0;
    private int _dataCollected = 0;

    public NordnetWebScraper(NordNetConfig config, ILogger<NordnetWebScraper> logger)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config), "Configuration cannot be empty!");
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Return Batch data from Nordnet API. Is used to retrive portfolio specific data.
    /// </summary>
    /// <param name="forceRun">Force execution and override refresh interval.</param>
    /// <param name="refreshIntervalMinutes">Refresh interval in minutes. If time is within interval, cched data are returned. Otherwise a request is being made to Nordnet.</param>
    /// <param name="headless">Indicate if Chrome should run in headless mode. Default true.</param>
    /// <returns>NordnetBatchData</returns>
    public async Task<NordnetBatchData> GetBatchData(bool forceRun = false, int refreshIntervalMinutes = 60, bool headless = true)
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
            Args = new[] { "--disable-dev-shm-usage", "--no-sandbox" },
            // ExecutablePath = "/usr/bin/chromium"
        };

        try
        {
            using var browser = await Puppeteer.LaunchAsync(options).ConfigureAwait(false);
            using var page = await browser.NewPageAsync().ConfigureAwait(false);
            await Login(page);
            page.Response += GetAccountIdEventHandler;
            _ = await page.GoToAsync($"https://www.nordnet.no/overview/details/", WaitUntilNavigation.Networkidle2);
            await WaitForConditionAsync(() => _accountId > 0, 60000, 100, $"Timed out while processing data from: https://www.nordnet.no/overview/details/");
            page.Response -= GetAccountIdEventHandler;

            page.Response += GetPortfolioEventHandler;
            _ = await page.GoToAsync($"https://www.nordnet.no/overview/details/{_accountId}", WaitUntilNavigation.Networkidle2);
            await WaitForConditionAsync(() => _dataCollected >= 2, 60000, 100, $"Timed out while processing data from: https://www.nordnet.no/overview/details/{_accountId}");
            page.Response -= GetPortfolioEventHandler;

            await browser.CloseAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while processing Nordnet data.");
        }
        return BatchData;
    }

    private async void GetAccountIdEventHandler(object? sender, ResponseCreatedEventArgs responseEvent)
    {
        var response = responseEvent.Response;

        if (!response.Ok)
        {
            _logger.LogWarning($"Response NOT OK: {response.Status}: {response.StatusText}");
            return;
        }

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
                        var txt = await response.TextAsync();
                        var accounts = JsonConvert.DeserializeObject<NordnetAccount[]>(txt);
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
    }

    private async void GetPortfolioEventHandler(object? sender, ResponseCreatedEventArgs responseEvent)
    {
        var response = responseEvent.Response;

        if (!response.Ok)
        {
            _logger.LogWarning($"Response NOT OK: {response.Status}: {response.StatusText}");
            return;
        }

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
                    var strBatch = JsonConvert.DeserializeObject<NordnetRequestStringBatch>(postDataText ?? string.Empty);
                    var postData = JsonConvert.DeserializeObject<NordnetRequest[]>(strBatch?.Batch ?? string.Empty);

                    if (postData?.GetType().IsArray == true)
                    {
                        for (var i = 0; i < postData.Length; i++)
                        {
                            if (postData[i].RelativeUrl.Contains($"accounts/{_accountId}/positions"))
                            {
                                _logger.LogInformation($"Found positions: {postData[i].RelativeUrl}");
                                var txt = await response.TextAsync();
                                var json = JsonConvert.DeserializeObject<NordnetJsonContent<NordnetPosition[]>[]>(txt);
                                _dataCollected = CollectPositions(json?[i].Body, _dataCollected);
                            }
                            else if (postData[i].RelativeUrl.Contains($"accounts/{_accountId}/info"))
                            {
                                _logger.LogInformation($"Found account info: {postData[i].RelativeUrl}");
                                var txt = await response.TextAsync();
                                var json = JsonConvert.DeserializeObject<NordnetJsonContent<NordnetAccountInfo[]>[]>(txt);
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
                        var json = JsonConvert.DeserializeObject<NordnetPosition[]>(txt);
                        _dataCollected = CollectPositions(json, _dataCollected);
                    }
                    else if (url?.Contains($"accounts/{_accountId}/info") == true)
                    {
                        var txt = await response.TextAsync();
                        var json = JsonConvert.DeserializeObject<NordnetAccountInfo[]>(txt);
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
        await page.WaitForXPathAsync("//button[contains(., 'innloggingsmetode')]", new() { Timeout = 10000 });
        var button1 = (await page.XPathAsync("//button[contains(., 'innloggingsmetode')]")).FirstOrDefault();
        if (button1 != null)
        {
            await button1.ClickAsync();
        }
        await page.WaitForXPathAsync("//button[contains(., 'brukernavn og passord')]", new() { Timeout = 10000 });
        var button2 = (await page.XPathAsync("//button[contains(., 'brukernavn og passord')]")).FirstOrDefault();
        if (button2 != null)
        {
            await button2.ClickAsync();
        }

        await page.WaitForSelectorAsync("input[name='username']", new() { Timeout = 10000 });
        await page.TypeAsync("input[name='username']", _config.Username);
        await page.TypeAsync("input[name='password']", _config.Password);

        Task.WaitAll(new[] {
                page.ClickAsync("button[type='submit']"),
                page.WaitForNavigationAsync()
            });
    }

    /// <summary>
    /// A general wait for method. It resolves when provided function returns true or if it times out.
    /// </summary>
    /// <param name="checkFn">The function to wait for. Return true to resolve the WaitFor method.</param>
    /// <param name="opts">Configuration options.</param>
    /// <returns></returns>
    private static Task<bool> WaitFor(Func<bool> checkFn, Option? opts = null)
    {
        opts ??= new Option(60000, 100, "Timeout!");
        var taskSource = new TaskCompletionSource<bool>();

        var ct = new CancellationTokenSource(opts.TimeoutMs);
        ct.Token.Register(() => taskSource.TrySetCanceled(), useSynchronizationContext: false);

        Task.Run(async () =>
        {
            while (true)
            {
                if (checkFn())
                {
                    taskSource.TrySetResult(true);
                    break; // Exit the loop when checkFn is true
                }
                await Task.Delay(opts.IntervalMs, ct.Token);
            }
        }, ct.Token);

        return taskSource.Task;
    }

    public async Task WaitForConditionAsync(Func<bool> condition, int timeoutMs = 60000, int intervalMs = 100, string timeoutMessage = "Timeout!")
    {
        _logger.LogDebug("Waiting for condition to be true...");
        var timer = new System.Timers.Timer(timeoutMs);
        var taskCompletionSource = new TaskCompletionSource<bool>();

        timer.Elapsed += (sender, args) =>
        {
            timer.Stop();
            _logger.LogDebug("Condition did not become true within the timeout period.");
            _logger.LogInformation(timeoutMessage);
            taskCompletionSource.TrySetResult(false);
        };

        timer.Start();

        while (!condition())
        {
            await Task.Delay(intervalMs);

            if (taskCompletionSource.Task.IsCompleted)
            {
                _logger.LogDebug("Condition check was canceled.");
                return;
            }
        }

        timer.Stop();
        _logger.LogDebug("Condition is now true.");
        taskCompletionSource.TrySetResult(true);
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