namespace Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits
{
    using System.Linq;

    using Domain.Core.Trading.Orders;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Factories.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    /// <summary>
    /// The fixed income high profits market closure rule.
    /// </summary>
    public class FixedIncomeHighProfitsMarketClosureRule : FixedIncomeHighProfitsStreamRule, IFixedIncomeHighProfitsMarketClosureRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIncomeHighProfitsMarketClosureRule"/> class.
        /// </summary>
        /// <param name="fixedIncomeParameters">
        /// The fixed income parameters.
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
        public FixedIncomeHighProfitsMarketClosureRule(
            IHighProfitsRuleFixedIncomeParameters fixedIncomeParameters,
            ISystemProcessOperationRunRuleContext ruleContext,
            ICostCalculatorFactory costCalculatorFactory,
            IRevenueCalculatorFactory revenueCalculatorFactory,
            IExchangeRateProfitCalculator exchangeRateProfitCalculator,
            IUniverseFixedIncomeOrderFilterService orderFilter,
            IUniverseEquityMarketCacheFactory equityMarketCacheFactory,
            IUniverseFixedIncomeMarketCacheFactory fixedIncomeMarketCacheFactory,
            IFixedIncomeMarketDataCacheStrategyFactory marketDataCacheFactory,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IFixedIncomeHighProfitJudgementService judgementService,
            RuleRunMode runMode,
            ILogger<FixedIncomeHighProfitsRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                fixedIncomeParameters,
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
        /// The cloning support for factor value brokerage.
        /// </summary>
        /// <param name="factorValue">
        /// The factor value.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseCloneableRule"/>.
        /// </returns>
        public override IUniverseCloneableRule Clone(IFactorValue factorValue)
        {
            var clone = (FixedIncomeHighProfitsMarketClosureRule)this.MemberwiseClone();
            clone.BaseClone();
            clone.OrganisationFactorValue = factorValue;

            return clone;
        }

        /// <summary>
        /// The run rule guard variant for market closure.
        /// </summary>
        /// <param name="history">
        /// The trading history.
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

            var tradeBuy = 
                activeWindow
                    .Where(aw => aw != null)
                    .Where(aw => aw.OrderDirection == OrderDirections.BUY || aw.OrderDirection == OrderDirections.COVER)
                    .ToList();

            var tradeSell =
                activeWindow
                    .Where(aw => aw != null)
                    .Where(aw => aw.OrderDirection == OrderDirections.SELL || aw.OrderDirection == OrderDirections.SHORT)
                    .ToList();

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
