using System;
using Microsoft.Extensions.Logging;
using Surveillance.Factories.Interfaces;
using Surveillance.Rules.Cancelled_Orders;
using Surveillance.Rules.Cancelled_Orders.Interfaces;
using Surveillance.Rule_Parameters;

namespace Surveillance.Factories
{
    public class CancelledOrderRuleFactory : ICancelledOrderRuleFactory
    {
        private readonly ICancelledOrderPositionDeDuplicator _deduplicator;
        private readonly ILogger<CancelledOrderRule> _logger;

        public CancelledOrderRuleFactory(
            ICancelledOrderPositionDeDuplicator deduplicator,
            ILogger<CancelledOrderRule> logger)
        {
            _deduplicator = deduplicator ?? throw new ArgumentNullException(nameof(deduplicator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ICancelledOrderRule Build()
        {
            var parameters = new CancelledOrderRuleParameters();

            return new CancelledOrderRule(_deduplicator, parameters, _logger);
        }
    }
}