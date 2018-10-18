using System;
using Microsoft.Extensions.Logging;
using Surveillance.Factories.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.HighVolume;
using Surveillance.Rules.HighVolume.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Factories
{
    public class HighVolumeRuleFactory : IHighVolumeRuleFactory
    {
        private readonly IHighVolumeMessageSender _messageSender;
        private readonly ILogger<IHighVolumeRule> _logger;

        public HighVolumeRuleFactory(IHighVolumeMessageSender messageSender, ILogger<IHighVolumeRule> logger)
        {
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IHighVolumeRule Build(IHighVolumeRuleParameters parameters, ISystemProcessOperationRunRuleContext opCtx)
        {
            return new HighVolumeRule(parameters, opCtx, _messageSender, _logger);
        }

        public string RuleVersion => Versioner.Version(1, 0);
    }
}
