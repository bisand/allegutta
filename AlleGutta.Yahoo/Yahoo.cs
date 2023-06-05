using System.Net;
using System.Web;
using AlleGutta.Models.Yahoo;
using Newtonsoft.Json;

namespace AlleGutta.Yahoo;
public sealed class YahooApi
{
    private readonly string scrapeUrl;
    private readonly string crumbUrl;
    private readonly string quotesUrl;
    private readonly string chartUrl;
    private readonly string optionsUrl;

    public YahooApi()
    {
        scrapeUrl = "https://finance.yahoo.com/quote/%5EGSPC/options";
        crumbUrl = "https://query1.finance.yahoo.com/v1/test/getcrumb";
        quotesUrl = "https://query2.finance.yahoo.com/v7/finance/quote";
        chartUrl = "https://query1.finance.yahoo.com/v8/finance/chart/";
        optionsUrl = "https://query2.finance.yahoo.com/v7/finance/options/";
    }

    private async Task<string> GetCrumbCookie()
    {
        var builder = new UriBuilder(scrapeUrl)
        {
            Port = -1
        };
        var query = HttpUtility.ParseQueryString(builder.Query);
        builder.Query = query.ToString();
        string url = builder.ToString();

        var cookies = new CookieContainer();
        using var handler = new HttpClientHandler { CookieContainer = cookies };
        using var client = new HttpClient(handler);
        var response = await client.GetAsync(url);
        var responseCookies = cookies.GetCookies(builder.Uri).Cast<Cookie>();

        return responseCookies.FirstOrDefault(c => c.Name == "B")?.Value ?? string.Empty;
    }

    private async Task<string> GetCrumb()
    {
        var builder = new UriBuilder(crumbUrl)
        {
            Port = -1
        };
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["formatted"] = "false";
        query["lang"] = "nb-NO";
        query["region"] = "NO";
        query["corsDomain"] = "finance.yahoo.com";
        builder.Query = query.ToString();
        string url = builder.ToString();

        var cookieContainer = new CookieContainer();
        using var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
        using var client = new HttpClient(handler);
        cookieContainer.Add(builder.Uri, new Cookie("B", await GetCrumbCookie()));
        var response = await client.GetStringAsync(url);

        return response ?? string.Empty;
    }

    public async Task<IEnumerable<QuoteResult>> GetQuotes(IEnumerable<string> tickers)
    {
        if (!tickers.Any()) return Array.Empty<QuoteResult>();

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
        query["crumb"] = await GetCrumb();
        builder.Query = query.ToString();
        string url = builder.ToString();

        using var client = new HttpClient();
        var response = await client.GetStringAsync(url);
        var quotes = JsonConvert.DeserializeObject<QuoteQyeryResult>(response, new[] { new InvalidDataFormatJsonConverter() });

        return quotes?.QuoteResponse?.Result ?? Array.Empty<QuoteResult>();
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
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["symbol"] = symbol;
        query["range"] = range;
        query["interval"] = interval;
        query["region"] = "NO";
        query["lang"] = "nb-NO";
        query["includePrePost"] = "false";
        query["events"] = "div|split|earn";
        query["corsDomain"] = "finance.yahoo.com";
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
