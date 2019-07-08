using System;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Rules.Interfaces;
using Surveillance.Engine.Rules.Judgements.Interfaces;
using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
using Surveillance.Engine.Rules.Queues.Interfaces;

namespace Surveillance.Engine.Rules.Judgements
{
    public class RuleViolationServiceFactory : IRuleViolationServiceFactory
    {
        private readonly IQueueCasePublisher _queueCasePublisher;
        private readonly IRuleBreachRepository _ruleBreachRepository;
        private readonly IRuleBreachOrdersRepository _ruleBreachOrdersRepository;
        private readonly IRuleBreachToRuleBreachOrdersMapper _ruleBreachToRuleBreachOrdersMapper;
        private readonly IRuleBreachToRuleBreachMapper _ruleBreachToRuleBreachMapper;
        private readonly ILogger<RuleViolationService> _logger;

        public RuleViolationServiceFactory(
            IQueueCasePublisher queueCasePublisher,
            IRuleBreachRepository ruleBreachRepository,
            IRuleBreachOrdersRepository ruleBreachOrdersRepository,
            IRuleBreachToRuleBreachOrdersMapper ruleBreachToRuleBreachOrdersMapper,
            IRuleBreachToRuleBreachMapper ruleBreachToRuleBreachMapper,
            ILogger<RuleViolationService> logger)
        {
            _queueCasePublisher =
                queueCasePublisher
                ?? throw new ArgumentNullException(nameof(queueCasePublisher));

            _ruleBreachRepository =
                ruleBreachRepository
                ?? throw new ArgumentNullException(nameof(ruleBreachRepository));

            _ruleBreachOrdersRepository =
                ruleBreachOrdersRepository 
                ?? throw new ArgumentNullException(nameof(ruleBreachOrdersRepository));

            _ruleBreachToRuleBreachOrdersMapper =
                ruleBreachToRuleBreachOrdersMapper 
                ?? throw new ArgumentNullException(nameof(ruleBreachToRuleBreachOrdersMapper));

            _ruleBreachToRuleBreachMapper =
                ruleBreachToRuleBreachMapper 
                ?? throw new ArgumentNullException(nameof(ruleBreachToRuleBreachMapper));

            _logger = 
                logger 
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public IRuleViolationService Build()
        {
            return new RuleViolationService(
                _queueCasePublisher,
                _ruleBreachRepository,
                _ruleBreachOrdersRepository,
                _ruleBreachToRuleBreachOrdersMapper,
                _ruleBreachToRuleBreachMapper,
                _logger);
        }
    }
}
