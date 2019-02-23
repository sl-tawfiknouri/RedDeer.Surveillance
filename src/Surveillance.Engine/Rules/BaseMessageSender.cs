using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService;
using Surveillance.DataLayer.Aurora.Rules.Interfaces;
using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
using Surveillance.Engine.Rules.Queues.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Rules
{
    public abstract class BaseMessageSender
    {
        private readonly IRuleBreachRepository _ruleBreachRepository;
        private readonly IRuleBreachOrdersRepository _ruleBreachOrdersRepository;
        private readonly IQueueCasePublisher _queueCasePublisher;
        private readonly IRuleBreachToRuleBreachOrdersMapper _ruleBreachToRuleBreachOrdersMapper;
        private readonly IRuleBreachToRuleBreachMapper _ruleBreachToRuleBreachMapper;
        private readonly string _messageSenderName;
        private readonly string _caseTitle;
        protected readonly ILogger Logger;

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
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _queueCasePublisher = queueCasePublisher ?? throw new ArgumentNullException(nameof(queueCasePublisher));
            _ruleBreachRepository = ruleBreachRepository ?? throw new ArgumentNullException(nameof(ruleBreachRepository));
            _ruleBreachOrdersRepository = ruleBreachOrdersRepository ?? throw new ArgumentNullException(nameof(ruleBreachOrdersRepository));
            _ruleBreachToRuleBreachOrdersMapper = ruleBreachToRuleBreachOrdersMapper ?? throw new ArgumentNullException(nameof(ruleBreachToRuleBreachOrdersMapper));
            _ruleBreachToRuleBreachMapper = ruleBreachToRuleBreachMapper ?? throw new ArgumentNullException(nameof(ruleBreachToRuleBreachMapper));

            _messageSenderName = messageSenderName ?? "unknown message sender";
            _caseTitle = caseTitle ?? "unknown rule breach detected";
        }

        protected async Task Send(IRuleBreach ruleBreach, string description)
        {
            if (ruleBreach?.Trades?.Get() == null)
            {
                Logger.LogInformation($"BaseMessageSender for {_messageSenderName} had null trades. Returning.");
                return;
            }

            Logger.LogInformation($"BaseMessageSender received message to send for {_messageSenderName} | security {ruleBreach.Security.Name}");

            var ruleBreachItem = _ruleBreachToRuleBreachMapper.RuleBreachItem(ruleBreach, description, _caseTitle);
            var ruleBreachId = await _ruleBreachRepository.Create(ruleBreachItem);

            if (ruleBreachId == null)
            {
                Logger.LogError($"{_messageSenderName} encountered an error saving the case message. Will not transmit to bus");
                return;
            }

            var ruleBreachOrderItems = _ruleBreachToRuleBreachOrdersMapper.ProjectToOrders(ruleBreach, ruleBreachId?.ToString());
            await _ruleBreachOrdersRepository.Create(ruleBreachOrderItems);
            var hasDuplicates = await _ruleBreachRepository.HasDuplicate(ruleBreachId?.ToString());

            if (hasDuplicates && !ruleBreach.IsBackTestRun)
            {
                Logger.LogInformation($"BaseMessageSender was going to send for {_messageSenderName} | security {ruleBreach.Security.Name} | rule breach {ruleBreachId} but detected duplicate case creation");
                return;
            }

            var caseMessage = new CaseMessage { RuleBreachId = ruleBreachId.GetValueOrDefault(0) };

            try
            {
                Logger.LogInformation($"BaseMessageSender about to send for {_messageSenderName} | security {ruleBreach.Security.Name}");
                await _queueCasePublisher.Send(caseMessage);
                Logger.LogInformation($"BaseMessageSender sent for {_messageSenderName} | security {ruleBreach.Security.Name}");
            }
            catch (Exception e)
            {
                Logger.LogError($"{_messageSenderName} encountered an error sending the case message to the bus {e}");
            }
        }
    }
}