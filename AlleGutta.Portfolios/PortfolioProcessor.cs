using AlleGutta.Nordnet;
using AlleGutta.Nordnet.Models;
using AlleGutta.Models.Portfolio;
using AlleGutta.Models.Yahoo;
using Microsoft.Extensions.Logging;

namespace AlleGutta.Portfolios;

public class PortfolioProcessor
{
    private readonly ILogger<PortfolioProcessor> _logger;

    public PortfolioProcessor(ILogger<PortfolioProcessor> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Portfolio GetPortfolioFromBatchData(string name, NordnetBatchData nordnetBatchData)
    {
        _logger.LogDebug("Generating '{name}' portfolio from batch data.", name);
        return new Portfolio()
        {
            Name = name,
            Ath = 0,
            Cash = nordnetBatchData.AccountInfo?.AccountSum?.Value ?? 0,
            MarketValue = nordnetBatchData.AccountInfo?.FullMarketvalue?.Value ?? 0,
            CostValue = nordnetBatchData.Positions?.Sum(x => x.Qty * x.AcqPrice?.Value ?? 0.0m) ?? 0.0m,
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

    public Portfolio UpdatePortfolioWithMarketData(Portfolio portfolio, IEnumerable<QuoteResult> quotes)
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
            _logger.LogDebug("Updating portfolio positions with market data.");
            var currentDay = DateTime.Now.Date;
            var newDay = false;

            // Reset portfolio values
            portfolio.ChangeTodayTotal = 0;
            portfolio.ChangeTodayPercent = 0;
            portfolio.ChangeTotal = 0;
            portfolio.ChangeTotalPercent = 0;
            portfolio.Equity = 0;
            portfolio.MarketValue = 0;
            portfolio.MarketValueMin = 0;
            portfolio.MarketValueMax = 0;
            portfolio.MarketValuePrev = 0;

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
                    result.ReturnValue = result.CurrentValue - result.CostValue;
                    result.ReturnPercent = result.CostValue != 0 ? result.ReturnValue / result.CostValue * 100 : 0;

                    portfolio.MarketValue += result.Shares * element.RegularMarketPrice ?? 0.0m;
                    portfolio.MarketValuePrev += result.Shares * element.RegularMarketPreviousClose ?? 0.0m;
                    portfolio.MarketValueMax += result.Shares * element.RegularMarketDayHigh ?? 0.0m;
                    portfolio.MarketValueMin += result.Shares * element.RegularMarketDayLow ?? 0.0m;
                    portfolio.ChangeTodayTotal += currentDay == symbolDay ? result.Shares * element.RegularMarketChange ?? 0.0m : 0.0m;
                }
                portfolio.Equity = portfolio.MarketValue + portfolio.Cash;
                if (portfolio.MarketValuePrev != 0)
                    portfolio.ChangeTodayPercent = portfolio.ChangeTodayTotal / portfolio.MarketValuePrev * 100;
                portfolio.ChangeTotal = portfolio.MarketValue - portfolio.CostValue;
                if (portfolio.CostValue != 0)
                    portfolio.ChangeTotalPercent = portfolio.ChangeTotal / portfolio.CostValue * 100;

                if (!newDay)
                {
                    portfolio.ChangeTodayTotal = 0.0m;
                    portfolio.ChangeTodayPercent = 0.0m;
                }
            }
            _logger.LogDebug("Successfully updated portfolio positions with market data.");
        }

        return portfolio;
    }
}