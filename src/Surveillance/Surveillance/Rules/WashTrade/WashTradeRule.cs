using System;
using Microsoft.Extensions.Logging;
using Surveillance.Rules.WashTrade.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades.Interfaces;
using Surveillance.Universe.MarketEvents;

namespace Surveillance.Rules.WashTrade
{
    public class WashTradeRule : BaseUniverseRule, IWashTradeRule
    {
        private int _alerts;
        private readonly ILogger _logger;

        public WashTradeRule(
            TimeSpan windowSize,
            ISystemProcessOperationRunRuleContext ruleCtx,
            ILogger logger)
            : base(
                windowSize,
                Domain.Scheduling.Rules.WashTrade,
                Versioner.Version(1, 0),
                "Wash Trade Rule",
                ruleCtx,
                logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void RunRule(ITradingHistoryStack history)
        {

        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
        }

        protected override void Genesis()
        {
            _logger.LogInformation("Genesis occured in the Wash Trade Rule");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation($"Eschaton occured in the Wash Trade Rule");

            RuleCtx?.UpdateAlertEvent(_alerts);
            RuleCtx?.EndEvent();
            _alerts = 0;
        }
    }
}
