using AlleGutta.Nordnet.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PuppeteerSharp;

namespace AlleGutta.Nordnet;

public class NordnetWebScraper
{
    private static readonly NordnetBatchData BatchData = new();
    private readonly NordNetConfig _config;
    private readonly ILogger<NordnetWebScraper> _logger;

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
        if (!forceRun && BatchData.CacheUpdated != null && DateTime.Now.AddMinutes(refreshIntervalMinutes * -1) > BatchData.CacheUpdated)
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

            var dataCollected = 0;
            page.Response += async (sender, responseEvent) =>
            {
                var response = responseEvent.Response;

                if (!response.Ok)
                {
                    _logger.LogWarning("Login response NOT OK", response.StatusText);
                    return;
                }

                try
                {
                    var request = response.Request;
                    var headers = response.Headers;

                    var url = request.Url;
                    var postDataText = (request.PostData ?? string.Empty).ToString();
                    var isAPI = url != null && (url.Contains("/api/2/batch") || url.Contains("/api/2/accounts"));
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
                                    if (postData[i].RelativeUrl.Contains("accounts/2/positions"))
                                    {
                                        var txt = await response.TextAsync();
                                        var json = JsonConvert.DeserializeObject<NordnetJsonContent<NordnetPosition[]>[]>(txt);
                                        dataCollected = CollectPositions(json?[i].Body, dataCollected);
                                    }
                                    else if (postData[i].RelativeUrl.Contains("accounts/2/info"))
                                    {
                                        var txt = await response.TextAsync();
                                        var json = JsonConvert.DeserializeObject<NordnetJsonContent<NordnetAccountInfo[]>[]>(txt);
                                        dataCollected = CollectAccountInfo(json?[i].Body?[0], dataCollected);
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
                            if (url?.Contains("accounts/2/positions") == true)
                            {
                                var txt = await response.TextAsync();
                                var json = JsonConvert.DeserializeObject<NordnetPosition[]>(txt);
                                dataCollected = CollectPositions(json, dataCollected);
                            }
                            else if (url?.Contains("accounts/2/info") == true)
                            {
                                var txt = await response.TextAsync();
                                var json = JsonConvert.DeserializeObject<NordnetAccountInfo[]>(txt);
                                dataCollected = CollectAccountInfo(json?[0], dataCollected);
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
            };
            Task.WaitAll(new[] {
                page.GoToAsync("https://www.nordnet.no/overview/details/2", WaitUntilNavigation.Networkidle0),
                WaitFor(() => dataCollected >= 2)
            });

            await browser.CloseAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while processing Nordnet data.");
        }
        return BatchData;
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
    private static Task WaitFor(Func<bool> checkFn, Option? opts = null)
    {
        opts ??= new Option(10000, 100, "Timeout!");
        var taskSource = new TaskCompletionSource<bool>();

        var ct = new CancellationTokenSource(opts.Timeout);
        ct.Token.Register(() => taskSource.TrySetCanceled(), useSynchronizationContext: false);

        Task.Run(async () =>
        {
            while (true)
            {
                if (checkFn())
                    taskSource.TrySetResult(true);
                await Task.Delay(opts.Interval, ct.Token);
            }
        }, ct.Token);

        return taskSource.Task;
    }

    private static int CollectAccountInfo(NordnetAccountInfo? json, int dataCollected)
    {
        BatchData.AccountInfo = json;
        BatchData.CacheUpdated = new DateTime();
        dataCollected++;
        return dataCollected;
    }

    private static int CollectPositions(NordnetPosition[]? json, int dataCollected)
    {
        BatchData.Positions = json;
        BatchData.CacheUpdated = new DateTime();
        dataCollected++;
        return dataCollected;
    }
}