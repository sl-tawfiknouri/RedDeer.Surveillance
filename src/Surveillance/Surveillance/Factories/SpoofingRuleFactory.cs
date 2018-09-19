using System;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.ElasticSearch.Rules.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.Rules.Spoofing;
using Surveillance.Rules.Spoofing.Interfaces;
using Surveillance.Rule_Parameters;

namespace Surveillance.Factories
{
    public class SpoofingRuleFactory : ISpoofingRuleFactory
    {
        private readonly IRuleBreachFactory _ruleBreachFactory;
        private readonly IRuleBreachRepository _ruleBreachRepository;
        private readonly ISpoofingRuleMessageSender _ruleMessageSender;
        private readonly ILogger<SpoofingRule> _logger;

        public SpoofingRuleFactory(
            IRuleBreachFactory ruleBreachFactory,
            IRuleBreachRepository ruleBreachRepository,
            ILogger<SpoofingRule> logger,
            ISpoofingRuleMessageSender ruleMessageSender)
        {
            _ruleBreachFactory = ruleBreachFactory ?? throw new ArgumentNullException(nameof(ruleBreachFactory));
            _ruleBreachRepository = ruleBreachRepository ?? throw new ArgumentNullException(nameof(ruleBreachRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ruleMessageSender = ruleMessageSender ?? throw new ArgumentNullException(nameof(ruleMessageSender));
        }

        public ISpoofingRule Build()
        {
            return new SpoofingRule(
                new SpoofingRuleParameters(),
                _ruleBreachFactory,
                _ruleBreachRepository,
                _ruleMessageSender,
                _logger);
        }
    }
}
