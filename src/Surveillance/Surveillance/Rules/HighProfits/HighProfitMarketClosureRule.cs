using System.Linq;
using DomainV2.Financial;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Data.Subscribers.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.Markets.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Interfaces;
using Surveillance.Rules.HighProfits.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;
using Surveillance.Universe.Filter.Interfaces;

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
            IExchangeRateProfitCalculator exchangeRateProfitCalculator,
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory factory,
            IMarketDataCacheStrategyFactory marketDataCacheFactory,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            RuleRunMode runMode,
            ILogger<HighProfitsRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                parameters, 
                ruleCtx,
                alertStream,
                costCalculatorFactory,
                revenueCalculatorFactory,
                exchangeRateProfitCalculator,
                orderFilter,
                factory,
                marketDataCacheFactory,
                dataRequestSubscriber,
                runMode,
                logger,
                tradingHistoryLogger)
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
                    .Where(aw => aw.OrderDirection == OrderDirections.BUY
                                 || aw.OrderDirection == OrderDirections.COVER)
                    .ToList();

            var tradeSell =
                activeWindow
                    .Where(aw => aw != null)
                    .Where(aw => 
                        aw.OrderDirection == OrderDirections.SELL
                        || aw.OrderDirection == OrderDirections.SHORT)
                    .ToList();

            var securitiesBrought = tradeBuy.Sum(tb => tb.OrderFilledVolume);
            var securitiesSold = tradeSell.Sum(tb => tb.OrderFilledVolume);

            if (securitiesBrought > securitiesSold)
            {
                Logger.LogInformation($"HighProfitMarketClosureRule RunRuleGuard securities brought {securitiesBrought} exceeded securities sold {securitiesSold}. Proceeding to evaluate market closure rule.");
                return true;
            }

            var position = new TradePosition(activeWindow.ToList());

            var message = new UniverseAlertEvent(DomainV2.Scheduling.Rules.HighProfits, position, _ruleCtx) { IsRemoveEvent = true };
            _alertStream.Add(message);

            Logger.LogInformation($"HighProfitMarketClosureRule RunRuleGuard securities brought {securitiesBrought} exceeded or equalled securities sold {securitiesSold}. Not proceeding to evaluate market closure rule.");

            return false;
        }

        public override object Clone()
        {
            var clone = (HighProfitMarketClosureRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }
    }
}
