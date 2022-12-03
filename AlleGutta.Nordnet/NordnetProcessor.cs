using System.Text.Json;
using PuppeteerSharp;

namespace AlleGutta.Nordnet;

public class NordnetProcessor
{
    private static NordnetBatchData BatchData = new NordnetBatchData();
    private readonly NordNetConfig _config;

    public NordnetProcessor(NordNetConfig config)
    {
        if (config is null)
            throw new ArgumentNullException(nameof(config), "Configuration cannot be empty!");
        _config = config;
    }

    public async Task<NordnetBatchData> GetBatchData(bool forceRun = false, int refreshIntervalMinutes = 60, bool headless = true)
    {
        if (!forceRun && BatchData.CacheUpdated != null && new DateTime().AddMinutes(refreshIntervalMinutes * -1) > BatchData.CacheUpdated)
        {
            return BatchData;
        }

        var options = new LaunchOptions
        {
            Headless = headless,
            DefaultViewport = { Width = 1024, Height = 768 },
            Args = new[] { "--disable-dev-shm-usage", "--no-sandbox" },
            ExecutablePath = "/usr/bin/chromium"
        };
        using (var browser = await Puppeteer.LaunchAsync(options))
        using (var page = await browser.NewPageAsync())
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

            page.Response += (sender, responseEvent) =>
            {
                var response = responseEvent.Response;

                if (!response.Ok)
                {
                    Console.WriteLine($"Response NOT OK: {response.StatusText}");
                    return;
                }

                var request = response.Request;
                var headers = response.Headers;

                var url = request.Url;
                var postDataText = request.PostData;
                var isAPI = url != null && (url.Contains("/api/2/batch") || url.Contains("/api/2/accounts"));
                var isPOST = request.Method == HttpMethod.Post;
                var isJson = headers.TryGetValue("content-type", out string? contentType) ? contentType != null ? contentType.Contains("application/json") : false : false;

                if (isAPI && isPOST && isJson)
                {
                    try
                    {
                        var batchData = JsonSerializer.Deserialize<dynamic>((string)postDataText);
                        var postData = JsonSerializer.Deserialize(batchData?.batch);
                        if (postData)
                        {
                            // for (let i = 0; i < postData.length; i++)
                            // {
                            //     const item = postData[i];
                            //     if (item.relative_url.includes('accounts/2/positions'))
                            //     {
                            //         const json = await response.json();
                            //         if (Array.isArray(json) && json.length > 0 && json[i]['body'] && Array.isArray(json[i]['body']))
                            //         {
                            //             dataCollected = await collectPositions(response, json[i]['body'], dataCollected);
                            //         }
                            //     }
                            //     else if (item.relative_url.includes('accounts/2/info'))
                            //     {
                            //         const json = await response.json();
                            //         if (Array.isArray(json) && json.length > 0 && json[i]['body'] && Array.isArray(json[i]['body']) && json[i]['body'].length > 0)
                            //         {
                            //             dataCollected = await collectAccountInfo(response, json[i]['body'][0], dataCollected);
                            //         }
                            //     }
                            // }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

            };

        }

        return null;
    }
}