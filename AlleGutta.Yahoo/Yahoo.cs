using System.Text.Json;
using System.Web;
using AlleGutta.Models;
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

    public async Task<Portfolio> GetPortfolio(Portfolio portfolio)
    {
        var tickers = "";
        decimal costValue = 0.0M;
        if (portfolio is null)
        {
            throw new ArgumentNullException(nameof(portfolio), "Portfolio positions cannot be null");
        }
        else
        {
            if (portfolio.Positions is null)
                return portfolio;

            foreach (var item in portfolio.Positions)
            {
                tickers += item.Symbol + ".OL,";
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
        Console.WriteLine(response);
        var quotes = JsonConvert.DeserializeObject<QuoteQyeryResult>(response, new[] { new InvalidDataFormatJsonConverter() });

        if (quotes?.QuoteResponse?.Result is not null)
        {
            var currentDay = DateTime.Now.Date;
            var newDay = false;
            foreach (var element in quotes.QuoteResponse.Result)
            {
                var symbol = element.Symbol?.TrimEnd(new[] { '.', 'O', 'L' });
                var symbolDate = element.RegularMarketTime;
                var symbolDay = (symbolDate ?? new DateTime()).Date;
                if (currentDay == symbolDay)
                {
                    newDay = true;
                }
                var result = Array.Find(portfolio.Positions, obj => obj.Symbol == symbol);
                if (result is not null)
                {
                    result.Name = element.LongName;
                    result.LastPrice = element.RegularMarketPrice ?? 0;
                    result.ChangeToday = currentDay == symbolDay ? element.RegularMarketChange ?? 0.0m : 0.0m;
                    result.ChangeTodayPercent = currentDay == symbolDay ? element.RegularMarketChangePercent ?? 0.0m : 0.0m;
                    result.PrevClose = element.RegularMarketPreviousClose ?? 0.0m;
                    result.CostValue = result.AvgPrice * result.Shares;
                    result.CurrentValue = result.LastPrice * result.Shares;
                    result.Return = result.CurrentValue - result.CostValue;
                    result.ReturnPercent = result.CostValue != 0 ? result.Return / result.CostValue * 100 : 0;

                    portfolio.MarketValue += result.Shares * element.RegularMarketPrice ?? 0.0m;
                    portfolio.MarketValuePrev += result.Shares * element.RegularMarketPreviousClose ?? 0.0m;
                    portfolio.MarketValueMax += result.Shares * element.RegularMarketDayHigh ?? 0.0m;
                    portfolio.MarketValueMin += result.Shares * element.RegularMarketDayLow ?? 0.0m;
                    portfolio.ChangeTodayTotal += currentDay == symbolDay ? result.Shares * element.RegularMarketChange ?? 0.0m : 0.0m;
                }
                portfolio.Equity = portfolio.MarketValue + portfolio.Cash;
                if (portfolio.MarketValuePrev != 0) portfolio.ChangeTodayPercent = portfolio.ChangeTodayTotal / portfolio.MarketValuePrev * 100;
                portfolio.ChangeTotal = portfolio.MarketValue - portfolio.CostValue;
                if (portfolio.CostValue != 0) portfolio.ChangeTotalPercent = portfolio.ChangeTotal / portfolio.CostValue * 100;

                if (!newDay)
                {
                    portfolio.ChangeTodayTotal = 0.0m;
                    portfolio.ChangeTodayPercent = 0.0m;
                }
            }
        }

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
