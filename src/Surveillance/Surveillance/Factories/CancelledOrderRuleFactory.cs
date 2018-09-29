using System;
using Microsoft.Extensions.Logging;
using Surveillance.Factories.Interfaces;
using Surveillance.Rules.Cancelled_Orders;
using Surveillance.Rules.Cancelled_Orders.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;

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

        public ICancelledOrderRule Build(ICancelledOrderRuleParameters parameters)
        {
            return new CancelledOrderRule(parameters, _messageSender, _logger);
        }
    }
}