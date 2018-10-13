using System;
using Microsoft.Extensions.Logging;
using Surveillance.Rules.Layering.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades.Interfaces;
using Surveillance.Universe.MarketEvents;

namespace Surveillance.Rules.Layering
{
    public class LayeringRule : BaseUniverseRule, ILayeringRule
    {
        private readonly ILogger _logger;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly ILayeringRuleParameters _parameters;
        private int _alertCount = 0;

        public LayeringRule(
            ILayeringRuleParameters parameters,
            ILogger logger,
            ISystemProcessOperationRunRuleContext opCtx)
            : base(
                parameters?.WindowSize ?? TimeSpan.FromMinutes(20),
                Domain.Scheduling.Rules.Layering,
                Versioner.Version(1, 0),
                "Layering Rule",
                logger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ruleCtx = opCtx ?? throw new ArgumentNullException(nameof(opCtx));
        }

        protected override void RunRule(ITradingHistoryStack history)
        {
        }

        protected override void Genesis()
        {
            _logger.LogInformation("Genesis occurred in the Layering Rule");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred in the Layering Rule at {exchange?.MarketOpen}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred in Layering Rule at {exchange?.MarketClose}");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation("Eschaton occured in Layering Rule");
            _ruleCtx.UpdateAlertEvent(_alertCount);
            _ruleCtx?.EndEvent();
            _alertCount = 0;
        }
    }
}
