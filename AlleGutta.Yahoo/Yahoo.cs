using System.Text.Json;
using System.Web;
using AlleGutta.Yahoo.Models;
using Newtonsoft.Json;

namespace AlleGutta.Yahoo;
public sealed class Yahoo
{
    private readonly string quotesUrl;
    private readonly string chartUrl;

    public Yahoo()
    {
        quotesUrl = "https://query2.finance.yahoo.com/v7/finance/quote";
        chartUrl = "https://query1.finance.yahoo.com/v8/finance/chart/";
    }

    public async Task<IEnumerable<QuoteResult>> GetQuotes(IEnumerable<string>? tickers)
    {
        var builder = new UriBuilder(quotesUrl)
        {
            Port = -1
        };
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["formatted"] = "false";
        query["lang"] = "nb-NO";
        query["region"] = "NO";
        query["symbols"] = tickers.Select(x => x).Aggregate((x, y) => $"{x},{y}");
        query["fields"] = "shortName,longName,regularMarketChange,regularMarketChangePercent,regularMarketTime,regularMarketPrice,regularMarketDayHigh,regularMarketDayRange,regularMarketDayLow,regularMarketVolume,regularMarketPreviousClose";
        query["corsDomain"] = "finance.yahoo.com";
        builder.Query = query.ToString();
        string url = builder.ToString();

        using var client = new HttpClient();
        var response = await client.GetStringAsync(url);
        Console.WriteLine(response);
        var quotes = JsonConvert.DeserializeObject<QuoteQyeryResult>(response, new[] { new InvalidDataFormatJsonConverter() });

        return quotes?.QuoteResponse?.Result ?? Array.Empty<QuoteResult>();
    }

    //     async getChartData(symbol: string, range: string, interval: string): Promise<object> {
    //             const searchParams = { symbol, range, interval, region: 'NO', lang: 'nb-NO', includePrePost: false, events: 'div|split|earn' };

    //     const chart = await got(chartUrl, { searchParams })
    //             .then(res =>
    //              {
    //     if (res)
    //     {
    //         return JSON.parse(res.body).chart.result;
    //     }
    // })
    //             .catch (err => {
    //             if (err && err.response)
    //             {
    //                 console.log(err.response.body);
    //                 return err.response.body;
    //             }
    //         });

    // if (chart && chart.length > 0)
    // {
    //     return chart[0];
    // }

    // return { };
    //         }

    //         savePortfolio(portfolio: Portfolio) {
    //     if (!portfolio) return;

    //     const portfolioPath = path.resolve('./data/portfolio_allegutta.json');
    //     const backupPortfolioPath = path.resolve('./data/portfolio_allegutta_backup_' + new Date().valueOf() + '.json');
    //     fs.copyFileSync(portfolioPath, backupPortfolioPath);
    //     fs.writeFileSync(portfolioPath, JSON.stringify(portfolio));
    //     portfolio = portfolio;
    //     return portfolio;
    // }
}
