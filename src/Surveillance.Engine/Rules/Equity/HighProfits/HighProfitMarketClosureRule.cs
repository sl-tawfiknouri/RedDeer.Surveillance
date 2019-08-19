﻿namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits
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
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Factories.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    public class HighProfitMarketClosureRule : HighProfitStreamRule, IHighProfitMarketClosureRule
    {
        public HighProfitMarketClosureRule(
            IHighProfitsRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            ICostCalculatorFactory costCalculatorFactory,
            IRevenueCalculatorFactory revenueCalculatorFactory,
            IExchangeRateProfitCalculator exchangeRateProfitCalculator,
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory marketCacheFactory,
            IMarketDataCacheStrategyFactory marketDataCacheFactory,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IHighProfitJudgementService judgementService,
            RuleRunMode runMode,
            ILogger<HighProfitsRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                equitiesParameters,
                ruleCtx,
                costCalculatorFactory,
                revenueCalculatorFactory,
                exchangeRateProfitCalculator,
                orderFilter,
                marketCacheFactory,
                marketDataCacheFactory,
                dataRequestSubscriber,
                judgementService,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            this.MarketClosureRule = true;
        }

        public override IUniverseCloneableRule Clone(IFactorValue factorValue)
        {
            var clone = (HighProfitMarketClosureRule)this.MemberwiseClone();
            clone.BaseClone();
            clone.OrganisationFactorValue = factorValue;

            return clone;
        }

        protected override bool RunRuleGuard(ITradingHistoryStack history)
        {
            var activeWindow = history.ActiveTradeHistory();

            if (!activeWindow.Any()) return false;

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