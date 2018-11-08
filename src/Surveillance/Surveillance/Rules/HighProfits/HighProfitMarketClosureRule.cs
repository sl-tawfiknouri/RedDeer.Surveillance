using System.Linq;
using Domain.Equity;
using Domain.Equity.Frames;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Currency.Interfaces;
using Surveillance.Rules.HighProfits.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.HighProfits
{
    public class HighProfitMarketClosureRule : HighProfitStreamRule, IHighProfitMarketClosureRule
    {
        private readonly IHighProfitRuleCachedMessageSender _messageSender;

        public HighProfitMarketClosureRule(
            ICurrencyConverter currencyConverter,
            IHighProfitRuleCachedMessageSender sender,
            IHighProfitsRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            ILogger<HighProfitsRule> logger)
            : base(currencyConverter, sender, parameters, ruleCtx, true, logger)
        {
            MarketClosureRule = true;
            _messageSender = sender;
        }

        protected override bool RunRuleGuard(ITradingHistoryStack history)
        {
            var activeWindow = history.ActiveTradeHistory();

            if (!activeWindow.Any())
            {
                return false;
            }

            var tradeBuy =
                activeWindow
                    .Where(aw => aw != null)
                    .Where(aw => aw.Position == OrderPosition.Buy)
                    .ToList();

            var tradeSell =
                activeWindow
                    .Where(aw => aw != null)
                    .Where(aw => aw.Position == OrderPosition.Sell)
                    .ToList();

            var securitiesBrought = tradeBuy.Sum(tb => tb.FulfilledVolume);
            var securitiesSold = tradeSell.Sum(tb => tb.FulfilledVolume);

            if (securitiesBrought > securitiesSold)
            {
                return true;
            }

            var position = new TradePosition(activeWindow.ToList());
            _messageSender.Remove(position);

            return false;
        }

        protected override Price? SecurityTickToPrice(SecurityTick tick)
        {
            if (tick == null)
            {
                return null;
            }

            return tick.IntradayPrices.Close ?? tick.Spread.Price;
        }
    }
}
