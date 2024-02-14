using System.Net;
using System.Web;
using AlleGutta.Models.Yahoo;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AlleGutta.Yahoo;
public sealed class YahooApi
{
    private readonly string scrapeUrl;
    private readonly string crumbUrl;
    private readonly string quotesUrl;
    private readonly string chartUrl;
    private readonly string optionsUrl;
    private readonly ILogger _logger;

    public YahooApi(ILoggerFactory loggerFactory)
    {
        scrapeUrl = "https://finance.yahoo.com/quote/%5EGSPC/options";
        crumbUrl = "https://query1.finance.yahoo.com/v1/test/getcrumb";
        quotesUrl = "https://query2.finance.yahoo.com/v7/finance/quote";
        chartUrl = "https://query1.finance.yahoo.com/v8/finance/chart/";
        optionsUrl = "https://query2.finance.yahoo.com/v7/finance/options/";
        CrumbManager.Logger = loggerFactory.CreateLogger("AlleGutta.Yahoo.CrumbManager");
        _logger = loggerFactory.CreateLogger<YahooApi>();
    }

    public async Task<IEnumerable<QuoteResult>> GetQuotes(IEnumerable<string> tickers, int requestTimeoutSeconds = 100, string? proxy = null)
    {
        if (!tickers.Any()) return [];

        var builder = new UriBuilder(quotesUrl)
        {
            Port = -1
        };
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["formatted"] = "false";
        query["lang"] = "nb-NO";
        query["region"] = "NO";
        query["symbols"] = tickers.Any() ? tickers.Select(x => x).Aggregate((x, y) => $"{x},{y}") : string.Empty;
        query["fields"] = "shortName,longName,regularMarketChange,regularMarketChangePercent,regularMarketTime,regularMarketPrice,regularMarketDayHigh,regularMarketDayRange,regularMarketDayLow,regularMarketVolume,regularMarketPreviousClose";
        query["corsDomain"] = "finance.yahoo.com";
        query["crumb"] = await CrumbManager.GetCrumbAsync(proxy);
        builder.Query = query.ToString();
        string url = builder.ToString();

        using var client = CrumbManager.GetHttpClient(proxy);
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        CrumbManager.FillHeaders(request, "https://finance.yahoo.com/quote/TSLA");

        client.Timeout = TimeSpan.FromSeconds(requestTimeoutSeconds);
        try
        {
            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var quotes = JsonConvert.DeserializeObject<QuoteQyeryResult>(responseBody, new[] { new InvalidDataFormatJsonConverter() });

            return quotes?.QuoteResponse?.Result ?? [];
        }
        // Filter by InnerException.
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            // Handle timeout.
            _logger.LogWarning($"Timeout when fetching quotes ({ex.Message}).");
        }
        catch (TaskCanceledException ex)
        {
            // Handle other task canceled exceptions.
            _logger.LogWarning($"Task canceled when fetching quotes ({ex.Message}).");
        }
        catch (HttpRequestException ex)
        {
            // Handle other http request exceptions.
            _logger.LogWarning($"Http request exception when fetching quotes ({ex.Message}).");
        }
        catch (Exception ex)
        {
            // Handle other exceptions.
            _logger.LogError(ex, "An error occurred when fetching quotes.");
        }
        return [];
    }

    public async Task<IEnumerable<ChartResult>> GetChartData(string symbol, string range, string interval)
    {
        // https://query1.finance.yahoo.com/v8/finance/chart/RECSI.OL
        // ?region=US&lang=en-US&includePrePost=false&interval=2m&useYfid=true&range=1d&corsDomain=finance.yahoo.com&.tsrc=finance
        // var searchParams = { symbol, range, interval, region: 'NO', lang: 'nb-NO', includePrePost: false, events: 'div|split|earn' };

        symbol = symbol.EndsWith(".OL", StringComparison.OrdinalIgnoreCase) ? symbol : $"{symbol.ToUpper()}.OL";
        var builder = new UriBuilder(chartUrl)
        {
            Port = -1
        };
        //https://query1.finance.yahoo.com/v8/finance/chart/TSLA?region=US&lang=en-US&includePrePost=false&interval=2m&useYfid=true&range=1d&corsDomain=finance.yahoo.com&.tsrc=finance

        var query = HttpUtility.ParseQueryString(builder.Query);
        query["symbol"] = symbol;
        query["range"] = range;
        query["interval"] = interval;
        query["region"] = "US";
        query["lang"] = "en-US";
        query["includePrePost"] = "false";
        query["events"] = "div|split|earn";
        query["corsDomain"] = "finance.yahoo.com";
        query[".tsrc"] = "finance";
        builder.Query = query.ToString();
        string url = builder.ToString();

        using var client = new HttpClient();
        var response = await client.GetStringAsync(url);
        var chart = JsonConvert.DeserializeObject<ChartQueryResult>(response, new[] { new InvalidDataFormatJsonConverter() });

        return chart?.Chart?.Result ?? Array.Empty<ChartResult>();
    }

    public async Task<OptionQuote?> GetInstrumentData(string symbol)
    {
        symbol = symbol.EndsWith(".OL", StringComparison.OrdinalIgnoreCase) ? symbol : $"{symbol.ToUpper()}.OL";
        var builder = new UriBuilder(optionsUrl + symbol)
        {
            Port = -1
        };
        string url = builder.ToString();

        using var client = new HttpClient();
        var response = await client.GetStringAsync(url);
        var optionRoot = JsonConvert.DeserializeObject<OptionRoot>(response, new[] { new InvalidDataFormatJsonConverter() });

        var result = optionRoot?.OptionChain?.Result?.Select(x => x.Quote);
        return result?.FirstOrDefault();
    }
}
