using System;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.ElasticSearch.Rules.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.Rules.Spoofing;
using Surveillance.Rules.Spoofing.Interfaces;

namespace Surveillance.Factories
{
    public class SpoofingRuleFactory : ISpoofingRuleFactory
    {
        private readonly IRuleBreachFactory _ruleBreachFactory;
        private readonly IRuleBreachRepository _ruleBreachRepository;
        private readonly ILogger<SpoofingRule> _logger;

        public SpoofingRuleFactory(
            IRuleBreachFactory ruleBreachFactory,
            IRuleBreachRepository ruleBreachRepository,
            ILogger<SpoofingRule> logger)
        {
            _ruleBreachFactory = ruleBreachFactory ?? throw new ArgumentNullException(nameof(ruleBreachFactory));
            _ruleBreachRepository = ruleBreachRepository ?? throw new ArgumentNullException(nameof(ruleBreachRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ISpoofingRule Build()
        {
            return new SpoofingRule(_ruleBreachFactory, _ruleBreachRepository, _logger);
        }
    }
}
