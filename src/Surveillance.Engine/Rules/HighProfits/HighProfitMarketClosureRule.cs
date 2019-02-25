using System.Linq;
using Domain.Financial;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Engine.Rules.Rules.HighProfits.Calculators.Interfaces;
using Surveillance.Engine.Rules.Rules.HighProfits.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

namespace Surveillance.Engine.Rules.Rules.HighProfits
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
                Logger.LogInformation($"RunRuleGuard securities brought {securitiesBrought} exceeded securities sold {securitiesSold}. Proceeding to evaluate market closure rule.");
                return true;
            }

            var position = new TradePosition(activeWindow.ToList());

            var message = new UniverseAlertEvent(Domain.Scheduling.Rules.HighProfits, position, _ruleCtx) { IsRemoveEvent = true };
            _alertStream.Add(message);

            Logger.LogInformation($"RunRuleGuard securities brought {securitiesBrought} exceeded or equalled securities sold {securitiesSold}. Not proceeding to evaluate market closure rule.");

            return false;
        }

        public override IUniverseCloneableRule Clone(IFactorValue factorValue)
        {
            var clone = (HighProfitMarketClosureRule)this.MemberwiseClone();
            clone.BaseClone();
            clone.OrganisationFactorValue = factorValue;

            return clone;
        }
    }
}
