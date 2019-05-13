using System;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.MarketEvents;

namespace Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute
{
    public class PlacingOrderWithNoIntentToExecuteRule : BaseUniverseRule, IPlacingOrdersWithNoIntentToExecuteRule
    {
        private readonly ILogger _logger;
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;

        public PlacingOrderWithNoIntentToExecuteRule(
            TimeSpan windowSize,
            IUniverseOrderFilter orderFilter,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseMarketCacheFactory marketCacheFactory,
            RuleRunMode runMode,
            ILogger logger,
            ILogger<TradingHistoryStack> tradingStackLogger) 
            : base(
                windowSize,
                Domain.Surveillance.Scheduling.Rules.PlacingOrderWithNoIntentToExecute,
                "STUB THIS OUT WITH THE FACTORY STATIC METHOD",
                "Placing Orders With No Intent To Execute Rule",
                ruleCtx,
                marketCacheFactory,
                runMode,
                logger,
                tradingStackLogger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ruleCtx = ruleCtx ?? throw new ArgumentNullException(nameof(ruleCtx));
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return _orderFilter.Filter(value);
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            // placing order with no intent to execute will not use this
        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
            // placing order with no intent to execute will not use this
        }

        protected override void Genesis()
        {
            _logger.LogInformation("Genesis occurred");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred at {exchange?.MarketOpen}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            // this will be used to pull in the orders for the day and check the sigma of limit order price level

            _logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred at {exchange?.MarketClose}");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation("Eschaton occurred");
            _ruleCtx?.EndEvent();
        }

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (PlacingOrderWithNoIntentToExecuteRule)Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        public object Clone()
        {
            var clone = (PlacingOrderWithNoIntentToExecuteRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }
    }
}
