namespace Surveillance.Engine.Rules.Judgements
{
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

    public class RuleViolationService : IRuleViolationService
    {
        private readonly Queue<RuleViolationIdPair> _deduplicatedRuleViolations;

        private readonly object _lock = new object();

        private readonly ILogger<RuleViolationService> _logger;

        private readonly IQueueCasePublisher _queueCasePublisher;

        private readonly IRuleBreachOrdersRepository _ruleBreachOrdersRepository;

        private readonly IRuleBreachRepository _ruleBreachRepository;

        private readonly IRuleBreachToRuleBreachMapper _ruleBreachToRuleBreachMapper;

        private readonly IRuleBreachToRuleBreachOrdersMapper _ruleBreachToRuleBreachOrdersMapper;

        private readonly Stack<IRuleBreach> _ruleViolations;

        public RuleViolationService(
            IQueueCasePublisher queueCasePublisher,
            IRuleBreachRepository ruleBreachRepository,
            IRuleBreachOrdersRepository ruleBreachOrdersRepository,
            IRuleBreachToRuleBreachOrdersMapper ruleBreachToRuleBreachOrdersMapper,
            IRuleBreachToRuleBreachMapper ruleBreachToRuleBreachMapper,
            ILogger<RuleViolationService> logger)
        {
            this._ruleViolations = new Stack<IRuleBreach>();
            this._deduplicatedRuleViolations = new Queue<RuleViolationIdPair>();

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

        public void AddRuleViolation(IRuleBreach ruleBreach)
        {
            if (ruleBreach == null)
            {
                this._logger?.LogError("received a null rule breach in add rule violations");
                return;
            }

            lock (this._lock)
            {
                this._ruleViolations.Push(ruleBreach);
            }
        }

        public void ProcessRuleViolationCache()
        {
            this.SaveRuleViolations();
            this.PublishRuleViolation();
        }

        private void PublishRuleViolation()
        {
            lock (this._lock)
            {
                foreach (var ruleViolation in this._deduplicatedRuleViolations)
                {
                    var caseMessage = new CaseMessage { RuleBreachId = ruleViolation.RuleViolationId };

                    try
                    {
                        this._logger.LogInformation(
                            $"about to send for {ruleViolation.RuleBreach.RuleParameterId} | security {ruleViolation.RuleBreach.Security.Name}");
                        this._queueCasePublisher.Send(caseMessage)
                            .Wait(); // prefer synchronrous send within a foreach loop
                        this._logger.LogInformation(
                            $"sent for {ruleViolation.RuleBreach.RuleParameterId} | security {ruleViolation.RuleBreach.Security.Name}");
                    }
                    catch (Exception e)
                    {
                        this._logger.LogError(e, $"{ruleViolation.RuleBreach.RuleParameterId} encountered an error sending the case message to the bus");
                    }
                }

                this._deduplicatedRuleViolations.Clear();
            }
        }

        private void SaveRuleViolations()
        {
            lock (this._lock)
            {
                while (this._ruleViolations.Any())
                {
                    var ruleViolation = this._ruleViolations.Pop();

                    if (ruleViolation == null) continue;

                    if (ruleViolation?.Trades?.Get() == null || (!ruleViolation?.Trades.Get().Any() ?? true))
                    {
                        this._logger.LogInformation($"{ruleViolation.RuleParameterId} had null trades. Returning.");
                        continue;
                    }

                    this._logger.LogInformation(
                        $"received message to send for {ruleViolation.RuleParameterId} | security {ruleViolation.Security.Name}");

                    // Save the rule breach
                    var ruleBreachItem = this._ruleBreachToRuleBreachMapper.RuleBreachItem(ruleViolation);
                    var ruleBreachIdTask = this._ruleBreachRepository.Create(ruleBreachItem);
                    ruleBreachIdTask.Wait();
                    var ruleBreachId = ruleBreachIdTask.Result;

                    if (ruleBreachId == null)
                    {
                        this._logger.LogError(
                            $"{ruleViolation?.RuleParameterId} encountered an error saving the case message. Will not transmit to bus");
                        continue;
                    }

                    // Save the rule breach orders
                    var ruleBreachOrderItems =
                        this._ruleBreachToRuleBreachOrdersMapper.ProjectToOrders(
                            ruleViolation,
                            ruleBreachId?.ToString());
                    this._ruleBreachOrdersRepository.Create(ruleBreachOrderItems).Wait();

                    // Check for duplicates
                    if (ruleViolation.IsBackTestRun)
                    {
                        var hasBackTestDuplicatesTask = this._ruleBreachRepository.HasDuplicateBackTest(
                            ruleBreachId?.ToString(),
                            ruleViolation.CorrelationId);
                        hasBackTestDuplicatesTask.Wait();

                        if (hasBackTestDuplicatesTask.Result)
                        {
                            this._logger.LogInformation(
                                $"was going to send for rule breach correlation id {ruleViolation.CorrelationId} | security {ruleViolation.Security.Name} | rule breach {ruleBreachId} but detected duplicate back test case creation");

                            continue;
                        }
                    }

                    var hasDuplicatesTask = this._ruleBreachRepository.HasDuplicate(ruleBreachId?.ToString());
                    hasDuplicatesTask.Wait();
                    var hasDuplicates = hasDuplicatesTask.Result;

                    if (hasDuplicates && !ruleViolation.IsBackTestRun)
                    {
                        this._logger.LogInformation(
                            $"was going to send for {ruleViolation.RuleParameterId} | security {ruleViolation.Security.Name} | rule breach {ruleBreachId} but detected duplicate case creation");
                        continue;
                    }

                    if (ruleViolation.RuleParameters.TunedParameters != null)
                    {
                        this._logger.LogInformation(
                            $"was going to send for {ruleViolation.RuleParameterId} | security {ruleViolation.Security.Name} | rule breach {ruleBreachId} but detected run was a tuning run");
                        continue;
                    }

                    var violationPair = new RuleViolationIdPair(ruleBreachId.GetValueOrDefault(0), ruleViolation);
                    this._deduplicatedRuleViolations.Enqueue(violationPair);
                }
            }
        }

        private class RuleViolationIdPair
        {
            public RuleViolationIdPair(long ruleViolationId, IRuleBreach ruleBreach)
            {
                this.RuleViolationId = ruleViolationId;
                this.RuleBreach = ruleBreach ?? throw new ArgumentNullException(nameof(ruleBreach));
            }

            public IRuleBreach RuleBreach { get; }

            public long RuleViolationId { get; }
        }
    }
}