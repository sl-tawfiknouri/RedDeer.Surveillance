using System;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.MarkingTheClose;
using Surveillance.Rules.MarkingTheClose.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Factories
{
    public class MarkingTheCloseRuleFactory : IMarkingTheCloseRuleFactory
    {
        private readonly ILogger<MarkingTheCloseRule> _logger;

        public MarkingTheCloseRuleFactory(ILogger<MarkingTheCloseRule> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IMarkingTheCloseRule Build(IMarkingTheCloseParameters parameters, ISystemProcessOperationRunRuleContext ruleCtx, IUniverseAlertStream alertStream)
        {
            return new MarkingTheCloseRule(parameters, alertStream, ruleCtx, _logger);
        }

        public static string Version => Versioner.Version(1, 0);
    }
}
