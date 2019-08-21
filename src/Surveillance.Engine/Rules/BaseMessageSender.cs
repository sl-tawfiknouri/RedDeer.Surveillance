namespace Surveillance.Engine.Rules.Rules
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService;

    using Surveillance.DataLayer.Aurora.Rules.Interfaces;
    using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
    using Surveillance.Engine.Rules.Queues.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    /// <summary>
    ///     Shared code before reaching the case queue publisher
    ///     Contains deduplication logic
    /// </summary>
    public abstract class BaseMessageSender
    {
        protected readonly ILogger Logger;

        private readonly string _caseTitle;

        private readonly string _messageSenderName;

        private readonly IQueueCasePublisher _queueCasePublisher;

        private readonly IRuleBreachOrdersRepository _ruleBreachOrdersRepository;

        private readonly IRuleBreachRepository _ruleBreachRepository;

        private readonly IRuleBreachToRuleBreachMapper _ruleBreachToRuleBreachMapper;

        private readonly IRuleBreachToRuleBreachOrdersMapper _ruleBreachToRuleBreachOrdersMapper;

        protected BaseMessageSender(
            string caseTitle,
            string messageSenderName,
            ILogger logger,
            IQueueCasePublisher queueCasePublisher,
            IRuleBreachRepository ruleBreachRepository,
            IRuleBreachOrdersRepository ruleBreachOrdersRepository,
            IRuleBreachToRuleBreachOrdersMapper ruleBreachToRuleBreachOrdersMapper,
            IRuleBreachToRuleBreachMapper ruleBreachToRuleBreachMapper)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

            this._caseTitle = caseTitle ?? string.Empty;
            this._messageSenderName = messageSenderName ?? "unknown message sender";
        }

        protected async Task Send(IRuleBreach ruleBreach, string description)
        {
            if (ruleBreach?.Trades?.Get() == null)
            {
                this.Logger.LogInformation($"{this._messageSenderName} had null trades. Returning.");
                return;
            }

            this.Logger.LogInformation(
                $"received message to send for {this._messageSenderName} | security {ruleBreach.Security.Name}");

            // Save the rule breach
            if (string.IsNullOrWhiteSpace(ruleBreach.CaseTitle)) ruleBreach.CaseTitle = this._caseTitle;

            if (string.IsNullOrWhiteSpace(ruleBreach.Description)) ruleBreach.Description = description;

            var ruleBreachItem = this._ruleBreachToRuleBreachMapper.RuleBreachItem(ruleBreach);
            var ruleBreachId = await this._ruleBreachRepository.Create(ruleBreachItem);

            if (ruleBreachId == null)
            {
                this.Logger.LogError(
                    $"{this._messageSenderName} encountered an error saving the case message. Will not transmit to bus");
                return;
            }

            // Save the rule breach orders
            var ruleBreachOrderItems =
                this._ruleBreachToRuleBreachOrdersMapper.ProjectToOrders(ruleBreach, ruleBreachId?.ToString());
            await this._ruleBreachOrdersRepository.Create(ruleBreachOrderItems);

            // Check for duplicates
            if (ruleBreach.IsBackTestRun)
            {
                var hasBackTestDuplicates = await this._ruleBreachRepository.HasDuplicateBackTest(
                                                ruleBreachId?.ToString(),
                                                ruleBreach.CorrelationId);

                if (hasBackTestDuplicates)
                {
                    this.Logger.LogInformation(
                        $"was going to send for {this._messageSenderName} | security {ruleBreach.Security.Name} | rule breach {ruleBreachId} but detected duplicate back test case creation");

                    return;
                }
            }

            var hasDuplicates = await this._ruleBreachRepository.HasDuplicate(ruleBreachId?.ToString());

            if (hasDuplicates && !ruleBreach.IsBackTestRun)
            {
                this.Logger.LogInformation(
                    $"was going to send for {this._messageSenderName} | security {ruleBreach.Security.Name} | rule breach {ruleBreachId} but detected duplicate case creation");
                return;
            }

            if (ruleBreach.RuleParameters.TunedParam != null)
            {
                this.Logger.LogInformation(
                    $"was going to send for {this._messageSenderName} | security {ruleBreach.Security.Name} | rule breach {ruleBreachId} but detected run was a tuning run");
                return;
            }

            var caseMessage = new CaseMessage { RuleBreachId = ruleBreachId.GetValueOrDefault(0) };

            try
            {
                this.Logger.LogInformation(
                    $"about to send for {this._messageSenderName} | security {ruleBreach.Security.Name}");
                await this._queueCasePublisher.Send(caseMessage);
                this.Logger.LogInformation($"sent for {this._messageSenderName} | security {ruleBreach.Security.Name}");
            }
            catch (Exception e)
            {
                this.Logger.LogError(
                    $"{this._messageSenderName} encountered an error sending the case message to the bus {e}");
            }
        }
    }
}