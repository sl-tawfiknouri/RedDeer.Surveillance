using System;
using Microsoft.Extensions.Logging;
using Surveillance.Factories.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.CancelledOrders;
using Surveillance.Rules.CancelledOrders.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Factories
{
    public class CancelledOrderRuleFactory : ICancelledOrderRuleFactory
    {
        private readonly ICancelledOrderRuleCachedMessageSender _messageSender;
        private readonly ILogger<CancelledOrderRule> _logger;

        public CancelledOrderRuleFactory(
            ICancelledOrderRuleCachedMessageSender messageSender,
            ILogger<CancelledOrderRule> logger)
        {
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ICancelledOrderRule Build(ICancelledOrderRuleParameters parameters, ISystemProcessOperationRunRuleContext ruleCtx)
        {
            return new CancelledOrderRule(parameters, _messageSender, ruleCtx, _logger);
        }

        public string Version => Versioner.Version(1, 0);
    }
}