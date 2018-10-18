using System;
using Microsoft.Extensions.Logging;
using Surveillance.Rules.HighVolume.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades.Interfaces;
using Surveillance.Universe.MarketEvents;

namespace Surveillance.Rules.HighVolume
{
    public class HighVolumeRule : BaseUniverseRule, IHighVolumeRule
    {
        private readonly IHighVolumeRuleParameters _parameters;
        private readonly ISystemProcessOperationRunRuleContext _opCtx;
        private readonly ILogger _logger;

        public HighVolumeRule(
            IHighVolumeRuleParameters parameters,
            ISystemProcessOperationRunRuleContext opCtx,
            ILogger<IHighVolumeRule> logger) 
            : base(
                parameters?.WindowSize ?? TimeSpan.FromDays(1),
                Domain.Scheduling.Rules.HighVolume,
                Versioner.Version(1, 0),
                "High Volume Rule",
                logger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _opCtx = opCtx ?? throw new ArgumentNullException(nameof(opCtx));
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
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {

        }

        protected override void MarketClose(MarketOpenClose exchange)
        {

        }

        protected override void EndOfUniverse()
        {

        }
    }
}
