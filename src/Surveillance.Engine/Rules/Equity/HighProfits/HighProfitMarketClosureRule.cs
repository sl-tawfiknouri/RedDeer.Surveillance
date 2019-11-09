namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits
{
    using System.Linq;

    using Domain.Core.Trading.Orders;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Factories.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    /// <summary>
    /// The high profit market closure rule.
    /// </summary>
    public class HighProfitMarketClosureRule : HighProfitStreamRule, IHighProfitMarketClosureRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HighProfitMarketClosureRule"/> class.
        /// </summary>
        /// <param name="equitiesParameters">
        /// The equities parameters.
        /// </param>
        /// <param name="ruleContext">
        /// The rule context.
        /// </param>
        /// <param name="costCalculatorFactory">
        /// The cost calculator factory.
        /// </param>
        /// <param name="revenueCalculatorFactory">
        /// The revenue calculator factory.
        /// </param>
        /// <param name="exchangeRateProfitCalculator">
        /// The exchange rate profit calculator.
        /// </param>
        /// <param name="orderFilter">
        /// The order filter.
        /// </param>
        /// <param name="equityMarketCacheFactory">
        /// The market cache factory.
        /// </param>
        /// <param name="fixedIncomeMarketCacheFactory">
        /// The market cache factory.
        /// </param>
        /// <param name="marketDataCacheFactory">
        /// The market data cache factory.
        /// </param>
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber.
        /// </param>
        /// <param name="judgementService">
        /// The judgement service.
        /// </param>
        /// <param name="runMode">
        /// The run mode.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="tradingHistoryLogger">
        /// The trading history logger.
        /// </param>
        public HighProfitMarketClosureRule(
            IHighProfitsRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext ruleContext,
            ICostCalculatorFactory costCalculatorFactory,
            IRevenueCalculatorFactory revenueCalculatorFactory,
            IExchangeRateProfitCalculator exchangeRateProfitCalculator,
            IUniverseOrderFilter orderFilter,
            IUniverseEquityMarketCacheFactory equityMarketCacheFactory,
            IUniverseFixedIncomeMarketCacheFactory fixedIncomeMarketCacheFactory,
            IEquityMarketDataCacheStrategyFactory marketDataCacheFactory,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IHighProfitJudgementService judgementService,
            RuleRunMode runMode,
            ILogger<HighProfitsRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                equitiesParameters,
                ruleContext,
                costCalculatorFactory,
                revenueCalculatorFactory,
                exchangeRateProfitCalculator,
                orderFilter,
                equityMarketCacheFactory,
                fixedIncomeMarketCacheFactory,
                marketDataCacheFactory,
                dataRequestSubscriber,
                judgementService,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            this.MarketClosureRule = true;
        }

        /// <summary>
        /// The clone.
        /// </summary>
        /// <param name="factorValue">
        /// The factor value.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseCloneableRule"/>.
        /// </returns>
        public override IUniverseCloneableRule Clone(IFactorValue factorValue)
        {
            var clone = (HighProfitMarketClosureRule)this.MemberwiseClone();
            clone.BaseClone();
            clone.OrganisationFactorValue = factorValue;

            return clone;
        }

        /// <summary>
        /// The run rule guard.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected override bool RunRuleGuard(ITradingHistoryStack history)
        {
            var activeWindow = history.ActiveTradeHistory();

            if (!activeWindow.Any())
            {
                return false;
            }

            var baseOrder = activeWindow.Any() ? activeWindow.Peek() : null;

            var tradeBuy = activeWindow.Where(aw => aw != null).Where(
                aw => aw.OrderDirection == OrderDirections.BUY || aw.OrderDirection == OrderDirections.COVER).ToList();

            var tradeSell = activeWindow.Where(aw => aw != null).Where(
                aw => aw.OrderDirection == OrderDirections.SELL || aw.OrderDirection == OrderDirections.SHORT).ToList();

            var securitiesBrought = tradeBuy.Sum(tb => tb.OrderFilledVolume);
            var securitiesSold = tradeSell.Sum(tb => tb.OrderFilledVolume);

            if (securitiesBrought > securitiesSold)
            {
                this.Logger.LogInformation(
                    $"RunRuleGuard securities brought {securitiesBrought} exceeded securities sold {securitiesSold}. Proceeding to evaluate market closure rule.");

                return true;
            }

            this.Logger.LogInformation(
                $"RunRuleGuard securities brought {securitiesBrought} exceeded or equaled securities sold {securitiesSold}. Not proceeding to evaluate market closure rule.");

            this.SetNoLiveTradesJudgement(baseOrder);

            return false;
        }
    }
}