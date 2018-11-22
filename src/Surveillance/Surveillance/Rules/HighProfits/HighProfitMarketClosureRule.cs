using System.Linq;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Currency.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Rules.HighProfits.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.HighProfits
{
    public class HighProfitMarketClosureRule : HighProfitStreamRule, IHighProfitMarketClosureRule
    {
        public HighProfitMarketClosureRule(
            IHighProfitsRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            ICostCalculatorFactory costCalculatorFactory,
            IRevenueCalculatorFactory revenueCalculatorFactory,
            ILogger<HighProfitsRule> logger)
            : base(
                parameters, 
                ruleCtx,
                alertStream,
                true,
                costCalculatorFactory,
                revenueCalculatorFactory,
                logger)
        {
            MarketClosureRule = true;
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

            var message = new UniverseAlertEvent(Domain.Scheduling.Rules.HighProfits, position, _ruleCtx) { IsRemoveEvent = true };
            _alertStream.Add(message);

            return false;
        }
    }
}
