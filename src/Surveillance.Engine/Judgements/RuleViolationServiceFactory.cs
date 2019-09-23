namespace Surveillance.Engine.Rules.Judgements
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.Rules.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
    using Surveillance.Engine.Rules.Queues.Interfaces;

    public class RuleViolationServiceFactory : IRuleViolationServiceFactory
    {
        private readonly ILogger<RuleViolationService> _logger;

        private readonly IQueueCasePublisher _queueCasePublisher;

        private readonly IRuleBreachOrdersRepository _ruleBreachOrdersRepository;

        private readonly IRuleBreachRepository _ruleBreachRepository;

        private readonly IRuleBreachToRuleBreachMapper _ruleBreachToRuleBreachMapper;

        private readonly IRuleBreachToRuleBreachOrdersMapper _ruleBreachToRuleBreachOrdersMapper;

        public RuleViolationServiceFactory(
            IQueueCasePublisher queueCasePublisher,
            IRuleBreachRepository ruleBreachRepository,
            IRuleBreachOrdersRepository ruleBreachOrdersRepository,
            IRuleBreachToRuleBreachOrdersMapper ruleBreachToRuleBreachOrdersMapper,
            IRuleBreachToRuleBreachMapper ruleBreachToRuleBreachMapper,
            ILogger<RuleViolationService> logger)
        {
            this._queueCasePublisher =
                queueCasePublisher ?? throw new ArgumentNullException(nameof(queueCasePublisher));

            this._ruleBreachRepository =
                ruleBreachRepository ?? throw new ArgumentNullException(nameof(ruleBreachRepository));

            this._ruleBreachOrdersRepository = ruleBreachOrdersRepository
                                               ?? throw new ArgumentNullException(nameof(ruleBreachOrdersRepository));

            this._ruleBreachToRuleBreachOrdersMapper = ruleBreachToRuleBreachOrdersMapper
                                                       ?? throw new ArgumentNullException(
                                                           nameof(ruleBreachToRuleBreachOrdersMapper));

            this._ruleBreachToRuleBreachMapper = ruleBreachToRuleBreachMapper
                                                 ?? throw new ArgumentNullException(
                                                     nameof(ruleBreachToRuleBreachMapper));

            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IRuleViolationService Build()
        {
            return new RuleViolationService(
                this._queueCasePublisher,
                this._ruleBreachRepository,
                this._ruleBreachOrdersRepository,
                this._ruleBreachToRuleBreachOrdersMapper,
                this._ruleBreachToRuleBreachMapper,
                this._logger);
        }
    }
}