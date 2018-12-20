using System;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.MarkingTheClose;
using Surveillance.Rules.MarkingTheClose.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Universe.Filter.Interfaces;

namespace Surveillance.Factories
{
    public class MarkingTheCloseRuleFactory : IMarkingTheCloseRuleFactory
    {
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly ILogger<MarkingTheCloseRule> _logger;

        public MarkingTheCloseRuleFactory(IUniverseOrderFilter orderFilter, ILogger<MarkingTheCloseRule> logger)
        {
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IMarkingTheCloseRule Build(IMarkingTheCloseParameters parameters, ISystemProcessOperationRunRuleContext ruleCtx, IUniverseAlertStream alertStream)
        {
            return new MarkingTheCloseRule(parameters, alertStream, ruleCtx, _orderFilter, _logger);
        }

        public static string Version => Versioner.Version(1, 0);
    }
}
