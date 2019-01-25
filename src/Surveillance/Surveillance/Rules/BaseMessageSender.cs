using System;
using System.Linq;
using System.Threading.Tasks;
using Contracts.SurveillanceService;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Rules.Interfaces;
using Surveillance.MessageBusIO.Interfaces;
using Surveillance.Rules.Interfaces;

namespace Surveillance.Rules
{
    public abstract class BaseMessageSender
    {
        private readonly IRuleBreachRepository _ruleBreachRepository;
        private readonly ICaseMessageSender _caseMessageSender;
        private readonly string _messageSenderName;
        private readonly string _caseTitle;
        protected readonly ILogger Logger;

        protected BaseMessageSender(
            string caseTitle,
            string messageSenderName,
            ILogger logger,
            ICaseMessageSender caseMessageSender,
            IRuleBreachRepository ruleBreachRepository)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _caseMessageSender = caseMessageSender ?? throw new ArgumentNullException(nameof(caseMessageSender));
            _ruleBreachRepository = ruleBreachRepository ?? throw new ArgumentNullException(nameof(ruleBreachRepository));

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

            var ruleBreachItem = RuleBreachItem(ruleBreach, description);
            var ruleBreachId = await _ruleBreachRepository.Create(ruleBreachItem);

            if (ruleBreachId == null)
            {
                Logger.LogError($"{_messageSenderName} encountered an error saving the case message. Will not transmit to bus");
                return;
            }

            var caseMessage = new CaseMessage { RuleBreachId = ruleBreachId.GetValueOrDefault(0) };

            try
            {
                Logger.LogInformation($"BaseMessageSender about to send for {_messageSenderName} | security {ruleBreach.Security.Name}");
                await _caseMessageSender.Send(caseMessage);
                Logger.LogInformation($"BaseMessageSender sent for {_messageSenderName} | security {ruleBreach.Security.Name}");
            }
            catch (Exception e)
            {
                Logger.LogError($"{_messageSenderName} encountered an error sending the case message to the bus {e}");
            }
        }

        private RuleBreach RuleBreachItem(IRuleBreach ruleBreach, string description)
        {
            var oldestPosition = ruleBreach.Trades?.Get()?.Min(tr => tr.MostRecentDateEvent());
            var latestPosition = ruleBreach.Trades?.Get()?.Max(tr => tr.MostRecentDateEvent());
            var venue = ruleBreach.Trades?.Get()?.FirstOrDefault()?.Market?.Name;

            if (oldestPosition == null)
            {
                oldestPosition = latestPosition;
            }

            if (latestPosition == null)
            {
                latestPosition = oldestPosition;
            }

            var oldestPositionValue = oldestPosition ?? DateTime.UtcNow;
            var latestPositionValue = latestPosition ?? DateTime.UtcNow;

            description = description ?? string.Empty;

            var trades =
                ruleBreach
                    .Trades
                    ?.Get()
                    ?.Select(io => io.ReddeerOrderId)
                    .Where(x => x.HasValue)
                    .Select(y => y.Value)
                    .ToList();

            var ruleBreachObj =
                new RuleBreach(
                    null,
                    ruleBreach.RuleParameterId,
                    null,
                    ruleBreach.IsBackTestRun,
                    DateTime.UtcNow,
                    _caseTitle,
                    description, 
                    venue,
                    oldestPositionValue,
                    latestPositionValue,
                    ruleBreach.Security.Cfi,
                    ruleBreach.Security.Identifiers.ReddeerEnrichmentId,
                    null,
                    trades);

            return ruleBreachObj;
        }
    }
}