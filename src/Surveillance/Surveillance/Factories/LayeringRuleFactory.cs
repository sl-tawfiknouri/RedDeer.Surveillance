using Surveillance.Rules;
using System;
using Surveillance.Rules.Layering;
using Surveillance.Rules.Layering.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Factories.Interfaces;

namespace Surveillance.Factories
{
    public class LayeringRuleFactory : ILayeringRuleFactory
    {
        private readonly ILayeringCachedMessageSender _messageSender;
        private readonly ILogger<LayeringRuleFactory> _logger;

        public LayeringRuleFactory(ILayeringCachedMessageSender messageSender, ILogger<LayeringRuleFactory> logger)
        {
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ILayeringRule Build(ILayeringRuleParameters parameters, ISystemProcessOperationRunRuleContext ruleCtx)
        {
            return new LayeringRule(parameters, _messageSender, _logger, ruleCtx);
        }

        public static string Version => Versioner.Version(1, 0);
    }
}
