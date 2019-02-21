using System;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.MarketEvents;

namespace Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits
{
    public class HighProfitsRule : BaseUniverseRule
    {
        public HighProfitsRule(
            TimeSpan windowSize,
            Domain.Scheduling.Rules rules,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseMarketCacheFactory marketCacheFactory,
            RuleRunMode runMode,
            ILogger<HighProfitsRule> logger,
            ILogger<TradingHistoryStack> tradingStackLogger)
            : base(
                windowSize,
                rules,
                Versioner.Version(1, 0),
                "Fixed Income High Profits Rule",
                ruleCtx,
                marketCacheFactory,
                runMode,
                logger,
                tradingStackLogger)
        {
        }

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            throw new NotImplementedException();
        }

        protected override void RunRule(ITradingHistoryStack history)
        {
            throw new NotImplementedException();
        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
            throw new NotImplementedException();
        }

        protected override void Genesis()
        {
            throw new NotImplementedException();
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            throw new NotImplementedException();
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            throw new NotImplementedException();
        }

        protected override void EndOfUniverse()
        {
            throw new NotImplementedException();
        }
    }
}
