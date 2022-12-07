using AlleGutta.Portfolios.Models;
using AlleGutta.Yahoo.Models;

namespace AlleGutta.Portfolios;

public static class PortfolioProcessor
{
    public static Portfolio? Process(Portfolio portfolio, IEnumerable<QuoteResult> quotes)
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
            var currentDay = DateTime.Now.Date;
            var newDay = false;
            foreach (var element in quotes)
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
}