using AlleGutta.Nordnet;
using AlleGutta.Nordnet.Models;
using AlleGutta.Portfolios.Models;
using AlleGutta.Yahoo.Models;
using Microsoft.Extensions.Logging;

namespace AlleGutta.Portfolios;

public class PortfolioProcessor
{
    private readonly ILogger<PortfolioProcessor> _logger;

    public PortfolioProcessor(ILogger<PortfolioProcessor> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Portfolio GetPortfolioFromBatchData(NordnetBatchData nordnetBatchData)
    {
        return new Portfolio()
        {
            Name = "AlleGutta",
            Ath = 0,
            Cash = nordnetBatchData.AccountInfo?.AccountSum?.Value ?? 0,
            MarketValue = nordnetBatchData.AccountInfo?.FullMarketvalue?.Value ?? 0,
            Positions = nordnetBatchData.Positions?.Select(pos =>
            {
                return new PortfolioPosition()
                {
                    Symbol = pos.Instrument?.Symbol,
                    Name = pos.Instrument?.Name,
                    Shares = (int)pos.Qty,
                    AvgPrice = pos.AcqPrice?.Value ?? 0
                };
            }).ToArray()
        };
    }

    public Portfolio Process(Portfolio portfolio, IEnumerable<QuoteResult> quotes)
    {
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
                costValue += item.Shares * item.AvgPrice;
            }
        }

        if (quotes is not null)
        {
            _logger.LogInformation("Updating portfolio positions with market data.");
            var currentDay = DateTime.Now.Date;
            var newDay = false;
            foreach (var element in quotes)
            {
                const string symbolSuffix = ".OL";
                var symbol = element.Symbol ?? string.Empty;
                if (element.Symbol?.EndsWith(symbolSuffix) == true)
                {
                    symbol = symbol[..symbol.LastIndexOf(symbolSuffix)];
                }

                var symbolDate = element.RegularMarketTime;
                var symbolDay = (symbolDate ?? new DateTime()).Date;
                if (currentDay == symbolDay)
                {
                    newDay = true;
                }
                var result = portfolio.Positions.FirstOrDefault(obj => obj
                    .Symbol?
                    .Equals(symbol, StringComparison.InvariantCultureIgnoreCase) == true);

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
            _logger.LogInformation("Successfully updated portfolio positions with market data.");
        }

        return portfolio;
    }
}