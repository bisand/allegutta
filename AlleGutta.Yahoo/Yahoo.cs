using System.Web;
using AlleGutta.Models;
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

    public async Task<Portfolio> GetPortfolio(IEnumerable<PortfolioPosition> positions)
    {
        var tickers = "";
        decimal costValue = 0.0M;
        if (positions is null)
        {
            throw new ArgumentNullException(nameof(positions), "Portfolio positions cannot be null");
        }
        else
        {
            foreach (var item in positions)
            {
                tickers += item.Symbol + ',';
                costValue += item.Shares * item.AvgPrice;
            }

            tickers = tickers.TrimEnd(',');
        }

        var builder = new UriBuilder(quotesUrl)
        {
            Port = -1
        };
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["formatted"] = "false";
        query["lang"] = "nb-NO";
        query["region"] = "NO";
        query["symbols"] = tickers;
        query["fields"] = "shortName,longName,regularMarketChange,regularMarketChangePercent,regularMarketTime,regularMarketPrice,regularMarketDayHigh,regularMarketDayRange,regularMarketDayLow,regularMarketVolume,regularMarketPreviousClose";
        query["corsDomain"] = "finance.yahoo.com";
        builder.Query = query.ToString();
        string url = builder.ToString();

        using var client = new HttpClient();
        var response = await client.GetStringAsync(url);
        var quotes = JsonConvert.DeserializeObject<dynamic>(response);

        var portfolio = new Portfolio();
        // if (quotes && Array.isArray(quotes))
        // {
        //     const currentDay = new Date().getDate();
        //     let newDay: boolean = false;
        //     quotes.forEach((element: { symbol: any; regularMarketTime: number; longName: string; regularMarketPrice: number; regularMarketChange: number; regularMarketChangePercent: number; regularMarketPreviousClose: number; regularMarketDayHigh: number; regularMarketDayLow: number; }) => {
        //         const symbol = element.symbol;
        //         const symbolDate = new Date(element.regularMarketTime * 1000);
        //         const symbolDay = symbolDate.getDate();
        //         if (currentDay === symbolDay)
        //         {
        //             newDay = true;
        //         }
        //         const result = portfolio.positions.find(obj =>
        //         {
        //             return obj.symbol === symbol;
        //         });
        //         if (result)
        //         {
        //             result.name = element.longName;
        //             result.last_price = element.regularMarketPrice;
        //             result.change_today = currentDay === symbolDay ? element.regularMarketChange : 0.0;
        //             result.change_today_percent = currentDay === symbolDay ? element.regularMarketChangePercent : 0.0;
        //             result.prev_close = element.regularMarketPreviousClose;
        //             result.cost_value = result.avg_price * result.shares;
        //             result.current_value = result.last_price * result.shares;
        //             result.return = result.current_value - result.cost_value;
        //             if (result.cost_value && result.cost_value !== 0)
        //             {
        //                 result.return_percent = (result.return / result.cost_value) *100;
        //             }
        //             else
        //             {
        //                 result.return_percent = 0;
        //             }

        //             portfolio.market_value += result.shares * element.regularMarketPrice;
        //             portfolio.market_value_prev += result.shares * element.regularMarketPreviousClose;
        //             portfolio.market_value_max += result.shares * element.regularMarketDayHigh;
        //             portfolio.market_value_min += result.shares * element.regularMarketDayLow;
        //             portfolio.change_today_total += currentDay === symbolDay ? result.shares * element.regularMarketChange : 0.0;
        //         }
        //         portfolio.equity = portfolio.market_value + portfolio.cash;
        //         portfolio.change_today_percent = (portfolio.change_today_total / portfolio.market_value_prev) * 100;
        //         portfolio.change_total = portfolio.market_value - portfolio.cost_value;
        //         portfolio.change_total_percent = (portfolio.change_total / portfolio.cost_value) * 100;

        //         if (!newDay)
        //         {
        //             portfolio.change_today_total = 0.0;
        //             portfolio.change_today_percent = 0.0;
        //         }
        //     });
        // }

        return portfolio;
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
