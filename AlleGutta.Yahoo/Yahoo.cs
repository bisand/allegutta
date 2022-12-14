using System.Web;
using AlleGutta.Yahoo.Models;
using Newtonsoft.Json;

namespace AlleGutta.Yahoo;
public sealed class YahooApi
{
    private readonly string quotesUrl;
    private readonly string chartUrl;

    public YahooApi()
    {
        quotesUrl = "https://query2.finance.yahoo.com/v7/finance/quote";
        chartUrl = "https://query1.finance.yahoo.com/v8/finance/chart/";
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
        builder.Query = query.ToString();
        string url = builder.ToString();

        using var client = new HttpClient();
        var response = await client.GetStringAsync(url);
        var quotes = JsonConvert.DeserializeObject<QuoteQyeryResult>(response, new[] { new InvalidDataFormatJsonConverter() });

        return quotes?.QuoteResponse?.Result ?? Array.Empty<QuoteResult>();
    }

    public async Task<IEnumerable<ChartResult>> GetChartData(string symbol, string range, string interval)
    {
        // var searchParams = { symbol, range, interval, region: 'NO', lang: 'nb-NO', includePrePost: false, events: 'div|split|earn' };
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
}
