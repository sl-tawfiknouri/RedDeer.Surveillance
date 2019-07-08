using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService;
using Surveillance.DataLayer.Aurora.Rules.Interfaces;
using Surveillance.Engine.Rules.Judgements.Interfaces;
using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
using Surveillance.Engine.Rules.Queues.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Judgements
{
    public class RuleViolationService : IRuleViolationService
    {
        private readonly IRuleBreachRepository _ruleBreachRepository;
        private readonly IRuleBreachOrdersRepository _ruleBreachOrdersRepository;
        private readonly IRuleBreachToRuleBreachOrdersMapper _ruleBreachToRuleBreachOrdersMapper;
        private readonly IRuleBreachToRuleBreachMapper _ruleBreachToRuleBreachMapper;

        private readonly object _lock = new object();
        private readonly Queue<IRuleBreach> _ruleViolations;
        private readonly Queue<RuleViolationIdPair> _deduplicatedRuleViolations;
        private readonly IQueueCasePublisher _queueCasePublisher;
        private readonly ILogger<RuleViolationService> _logger;

        public RuleViolationService(
            IQueueCasePublisher queueCasePublisher,
            IRuleBreachRepository ruleBreachRepository,
            IRuleBreachOrdersRepository ruleBreachOrdersRepository,
            IRuleBreachToRuleBreachOrdersMapper ruleBreachToRuleBreachOrdersMapper,
            IRuleBreachToRuleBreachMapper ruleBreachToRuleBreachMapper,
            ILogger<RuleViolationService> logger)
        {
            _ruleViolations = new Queue<IRuleBreach>();
            _deduplicatedRuleViolations = new Queue<RuleViolationIdPair>();

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

        public void AddRuleViolation(IRuleBreach ruleBreach)
        {
            if (ruleBreach == null)
            {
                _logger?.LogError($"received a null rule breach in add rule violations");
            }

            lock (_lock)
            {
                _ruleViolations.Enqueue(ruleBreach);
            }
        }

        public void ProcessRuleViolationCache()
        {
            SaveRuleViolations();
            PublishRuleViolation();
        }

        private void SaveRuleViolations()
        {
            lock (_lock)
            {
                while (_ruleViolations.Any())
                {
                    var ruleViolation = _ruleViolations.Dequeue();

                    if (ruleViolation == null)
                        continue;

                    if (ruleViolation?.Trades?.Get() == null)
                    {
                        _logger.LogInformation($"{ruleViolation.RuleParameterId} had null trades. Returning.");
                        return;
                    }

                    _logger.LogInformation($"received message to send for {ruleViolation.RuleParameterId} | security {ruleViolation.Security.Name}");

                    // Save the rule breach
                    var ruleBreachItem = _ruleBreachToRuleBreachMapper.RuleBreachItem(ruleViolation);
                    var ruleBreachIdTask = _ruleBreachRepository.Create(ruleBreachItem);
                    ruleBreachIdTask.Wait();
                    var ruleBreachId = ruleBreachIdTask.Result;

                    if (ruleBreachId == null)
                    {
                        _logger.LogError($"{ruleViolation.RuleParameterId} encountered an error saving the case message. Will not transmit to bus");
                        return;
                    }

                    // Save the rule breach orders
                    var ruleBreachOrderItems = _ruleBreachToRuleBreachOrdersMapper.ProjectToOrders(ruleViolation, ruleBreachId?.ToString());
                    _ruleBreachOrdersRepository.Create(ruleBreachOrderItems).Wait();

                    // Check for duplicates
                    var hasDuplicatesTask = _ruleBreachRepository.HasDuplicate(ruleBreachId?.ToString());
                    hasDuplicatesTask.Wait();
                    var hasDuplicates = hasDuplicatesTask.Result;

                    if (hasDuplicates && !ruleViolation.IsBackTestRun)
                    {
                        _logger.LogInformation($"was going to send for {ruleViolation.RuleParameterId} | security {ruleViolation.Security.Name} | rule breach {ruleBreachId} but detected duplicate case creation");
                        return;
                    }

                    if (ruleViolation.RuleParameters.TunedParam != null)
                    {
                        _logger.LogInformation($"was going to send for {ruleViolation.RuleParameterId} | security {ruleViolation.Security.Name} | rule breach {ruleBreachId} but detected run was a tuning run");
                        return;
                    }

                    var violationPair = new RuleViolationIdPair(ruleBreachId.GetValueOrDefault(0), ruleViolation);
                    _deduplicatedRuleViolations.Enqueue(violationPair);
                }
            }
        }

        private void PublishRuleViolation()
        {
            lock (_lock)
            {
                foreach (var ruleViolation in _deduplicatedRuleViolations)
                {
                    var caseMessage = new CaseMessage { RuleBreachId = ruleViolation.RuleViolationId };

                    try
                    {
                        _logger.LogInformation($"about to send for {ruleViolation.RuleBreach.RuleParameterId} | security {ruleViolation.RuleBreach.Security.Name}");
                        _queueCasePublisher.Send(caseMessage).Wait(); // prefer synchronrous send within a foreach loop
                        _logger.LogInformation($"sent for {ruleViolation.RuleBreach.RuleParameterId} | security {ruleViolation.RuleBreach.Security.Name}");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"{ruleViolation.RuleBreach.RuleParameterId} encountered an error sending the case message to the bus {e}");
                    }
                }

                _deduplicatedRuleViolations.Clear();
            }
        }

        private class RuleViolationIdPair
        {
            public RuleViolationIdPair(
                long ruleViolationId, 
                IRuleBreach ruleBreach)
            {
                RuleViolationId = ruleViolationId;
                RuleBreach = ruleBreach ?? throw new ArgumentNullException(nameof(ruleBreach));
            }

            public long RuleViolationId { get; }
            public IRuleBreach RuleBreach { get;  }
        }
    }
}
