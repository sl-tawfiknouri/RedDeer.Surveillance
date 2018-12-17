﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DomainV2.Scheduling;
using DomainV2.Scheduling.Interfaces;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Interfaces;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.Scheduler.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Utilities.Aws_IO;
using Utilities.Aws_IO.Interfaces;
using Utilities.Extensions;

namespace Surveillance.Scheduler
{
    public class ReddeerDistributedRuleScheduler : IReddeerDistributedRuleScheduler
    {
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly IScheduledExecutionMessageBusSerialiser _messageBusSerialiser;
        private readonly IRuleParameterApiRepository _ruleParameterApiRepository;
        private readonly ISystemProcessContext _systemProcessContext;

        private readonly ILogger<ReddeerDistributedRuleScheduler> _logger;
        private CancellationTokenSource _messageBusCts;
        private AwsResusableCancellationToken _token;

        public ReddeerDistributedRuleScheduler(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IScheduledExecutionMessageBusSerialiser messageBusSerialiser,
            IRuleParameterApiRepository ruleParameterApiRepository,
            ISystemProcessContext systemProcessContext,
            ILogger<ReddeerDistributedRuleScheduler> logger)
        {
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            _messageBusSerialiser = messageBusSerialiser ?? throw new ArgumentNullException(nameof(messageBusSerialiser));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _ruleParameterApiRepository =
                ruleParameterApiRepository
                ?? throw new ArgumentNullException(nameof(ruleParameterApiRepository));

            _systemProcessContext =
                systemProcessContext
                ?? throw new ArgumentNullException(nameof(systemProcessContext));
        }

        public void Initiate()
        {
            _messageBusCts?.Cancel();
            _messageBusCts = new CancellationTokenSource();
            _token = new AwsResusableCancellationToken();

            _awsQueueClient.SubscribeToQueueAsync(
                _awsConfiguration.ScheduledRuleQueueName,
                async (s1, s2) => { await ExecuteNonDistributedMessage(s1, s2); },
                _messageBusCts.Token,
                _token);
        }

        public void Terminate()
        {
            _messageBusCts?.Cancel();
            _messageBusCts = null;
        }

        public async Task ExecuteNonDistributedMessage(string messageId, string messageBody)
        {
            var opCtx = _systemProcessContext.CreateAndStartOperationContext();

            _logger.LogInformation($"ReddeerRuleScheduler read message {messageId} with body {messageBody} from {_awsConfiguration.ScheduledRuleQueueName}");

            var execution = _messageBusSerialiser.DeserialisedScheduledExecution(messageBody);

            if (execution == null)
            {
                _logger.LogError($"ReddeerRuleScheduler was unable to deserialise the message {messageId}");
                opCtx.EndEventWithError($"ReddeerRuleScheduler was unable to deserialise the message {messageId}");
                return;
            }

            if (execution?.Rules == null
                || !execution.Rules.Any())
            {
                _logger.LogError($"ReddeerRuleScheduler deserialised message {messageId} but could not find any rules on the scheduled execution");
                opCtx.EndEventWithError($"ReddeerRuleScheduler deserialised message {messageId} but could not find any rules on the scheduled execution");
                return;
            }

            var parameters = await _ruleParameterApiRepository.Get();
            var ruleCtx = BuildRuleCtx(opCtx, execution);
            await ScheduleRule(execution, parameters, ruleCtx);

            ruleCtx
                .EndEvent()
                .EndEvent();
        }

        private async Task ScheduleRule(
            ScheduledExecution execution,
            RuleParameterDto parameters,
            ISystemProcessOperationDistributeRuleContext ruleCtx)
        {
            foreach (var rule in execution.Rules.Where(ru => ru != null))
            {
                switch (rule.Rule)
                {
                    case DomainV2.Scheduling.Rules.CancelledOrders:
                        // var cancelledOrderRuleRuns = parameters.CancelledOrders?.Select(co => co as IIdentifiableRule)?.ToList();
                        // await ScheduleRuleRuns(execution, cancelledOrderRuleRuns, rule, ruleCtx);
                        break;
                    case DomainV2.Scheduling.Rules.HighProfits:
                        var highProfitRuleRuns = parameters.HighProfits?.Select(co => co as IIdentifiableRule)?.ToList();
                        await ScheduleRuleRuns(execution, highProfitRuleRuns, rule, ruleCtx);
                        break;
                    case DomainV2.Scheduling.Rules.HighVolume:
                        var highVolumeRuleRuns = parameters.HighVolumes?.Select(co => co as IIdentifiableRule)?.ToList();
                        await ScheduleRuleRuns(execution, highVolumeRuleRuns, rule, ruleCtx);
                        break;
                    case DomainV2.Scheduling.Rules.Layering:
                        // var layeringRuleRuns = parameters.Layerings?.Select(co => co as IIdentifiableRule)?.ToList();
                        // await ScheduleRuleRuns(execution, layeringRuleRuns, rule, ruleCtx);
                        break;
                    case DomainV2.Scheduling.Rules.MarkingTheClose:
                        // var markingTheCloseRuleRuns = parameters.MarkingTheCloses?.Select(co => co as IIdentifiableRule)?.ToList();
                        // await ScheduleRuleRuns(execution, markingTheCloseRuleRuns, rule, ruleCtx);
                        break;
                    case DomainV2.Scheduling.Rules.Spoofing:
                        // var spoofingRuleRuns = parameters.Spoofings?.Select(co => co as IIdentifiableRule)?.ToList();
                        // await ScheduleRuleRuns(execution, spoofingRuleRuns, rule, ruleCtx);
                        break;
                    case DomainV2.Scheduling.Rules.WashTrade:
                        var washTradeRuleRuns = parameters.WashTrades?.Select(co => co as IIdentifiableRule)?.ToList();
                        await ScheduleRuleRuns(execution, washTradeRuleRuns, rule, ruleCtx);
                        break;
                    case DomainV2.Scheduling.Rules.UniverseFilter:
                        break;
                    default:
                        _logger.LogError($"{rule.Rule} was scheduled but not recognised by the Schedule Rule method in distributed rule.");
                        ruleCtx.EventError($"{rule.Rule} was scheduled but not recognised by the Schedule Rule method in distributed rule.");
                        break;
                }
            }
        }

        private async Task ScheduleRuleRuns(
            ScheduledExecution execution, 
            IReadOnlyCollection<IIdentifiableRule> identifiableRules,
            RuleIdentifier rule,
            ISystemProcessOperationDistributeRuleContext ruleCtx)
        {
            if (identifiableRules == null
                || !identifiableRules.Any())
            {
                _logger.LogWarning($"Scheduled rule execution did not have any identifiable rules for rule {rule.Rule}");
                return;
            }

            if (rule.Ids == null || !rule.Ids.Any())
            {
                foreach (var ruleSet in identifiableRules)
                {
                    var ruleInstance = new RuleIdentifier { Rule = rule.Rule, Ids = new[] { ruleSet.Id } };
                    await ScheduleRule(ruleInstance, execution, ruleSet.WindowSize, ruleCtx.Id);
                }
            }
            else
            {
                foreach (var id in rule.Ids)
                {
                    var identifiableRule =
                        identifiableRules
                            ?.FirstOrDefault(param =>
                                string.Equals(param.Id, id, StringComparison.InvariantCultureIgnoreCase));

                    if (identifiableRule == null)
                    {
                        _logger.LogError($"Reddeer Distributed Rule Scheduler asked to schedule an execution for rule {rule.Rule.GetDescription()} with id of {id} which was not found when querying the rule parameter API on the client service.");

                        ruleCtx.EventError($"Reddeer Distributed Rule Scheduler asked to schedule an execution for rule {rule.Rule.GetDescription()} with id of {id} which was not found when querying the rule parameter API on the client service.");

                        continue;
                    }

                    var ruleInstance = new RuleIdentifier { Rule = rule.Rule, Ids = new[] { id } };
                    await ScheduleRule(ruleInstance, execution, identifiableRule.WindowSize, ruleCtx.Id);
                }
            }
        }

        private ISystemProcessOperationDistributeRuleContext BuildRuleCtx(
            ISystemProcessOperationContext opCtx,
            ScheduledExecution execution)
        {
            return opCtx
                    .CreateAndStartDistributeRuleContext(
                        execution.TimeSeriesInitiation.DateTime,
                        execution.TimeSeriesTermination.DateTime,
                        execution.Rules?.Aggregate(string.Empty, (x, i) => x + ", " + i.Rule.GetDescription() + $" ({(i.Ids.Any() ? i.Ids?.Aggregate((a, b) => a + "," + b)?.Trim(',') ?? string.Empty : string.Empty) })").Trim(',', ' '));
        }

        private async Task ScheduleRule(RuleIdentifier ruleIdentifier, ScheduledExecution execution, TimeSpan? timespan, string correlationId)
        {
            var ruleTimespan = execution.TimeSeriesTermination - execution.TimeSeriesInitiation;
            var ruleParameterTimeWindow = timespan;

            if (ruleParameterTimeWindow == null
                || ruleTimespan.TotalDays < 7)
            {
                await ScheduleSingleExecution(ruleIdentifier, execution, correlationId);
                return;
            }

            if (ruleParameterTimeWindow.GetValueOrDefault().TotalDays >= ruleTimespan.TotalDays)
            {
                await ScheduleSingleExecution(ruleIdentifier, execution, correlationId);
                return;
            }

            var daysToRunRuleFor = Math.Max(6, ruleParameterTimeWindow.GetValueOrDefault().TotalDays * 3);

            if (daysToRunRuleFor >= ruleTimespan.TotalDays)
            {
                await ScheduleSingleExecution(ruleIdentifier, execution, correlationId);
                return;
            }

            await TimeSplitter(ruleIdentifier, execution, ruleParameterTimeWindow, daysToRunRuleFor, correlationId);
        }

        private async Task ScheduleSingleExecution(RuleIdentifier rule, ScheduledExecution execution, string correlationId)
        {
            var distributedExecution = new ScheduledExecution
            {
                Rules = new List<RuleIdentifier> { rule },
                TimeSeriesInitiation = execution.TimeSeriesInitiation,
                TimeSeriesTermination = execution.TimeSeriesTermination,
                CorrelationId = correlationId,
                IsBackTest = execution.IsBackTest
            };

            await ScheduleExecution(distributedExecution);
        }

        private async Task TimeSplitter(
            RuleIdentifier rule,
            ScheduledExecution execution,
            TimeSpan? ruleParameterTimeWindow,
            double daysToRunRuleFor,
            string correlationId)
        {
            var continueSplit = true;
            var executions = new List<ScheduledExecution>();
            var currentInitiationPoint = execution.TimeSeriesInitiation;

            while (continueSplit)
            {
                var currentEndPoint = currentInitiationPoint.AddDays(daysToRunRuleFor);
                if (currentEndPoint > execution.TimeSeriesTermination)
                {
                    currentEndPoint = execution.TimeSeriesTermination;
                }

                var distributedExecution = new ScheduledExecution
                {
                    Rules = new List<RuleIdentifier> { rule },
                    TimeSeriesInitiation = currentInitiationPoint,
                    TimeSeriesTermination = currentEndPoint,
                    CorrelationId = correlationId,
                    IsBackTest = execution.IsBackTest
                };

                executions.Add(distributedExecution);
                currentInitiationPoint = currentEndPoint.AddDays(1);

                if (currentInitiationPoint > execution.TimeSeriesTermination)
                {
                    continueSplit = false;
                }

                currentInitiationPoint = currentInitiationPoint.AddDays(-ruleParameterTimeWindow.GetValueOrDefault().TotalDays);
            }

            foreach (var executionSplit in executions)
            {
                await ScheduleExecution(executionSplit);
            }
        }

        private async Task ScheduleExecution(ScheduledExecution distributedExecution)
        {
            var serialisedDistributedExecution =
                _messageBusSerialiser.SerialiseScheduledExecution(distributedExecution);

            _logger.LogInformation($"Reddeer Smart Rule Scheduler - dispatching distribute message to queue - {serialisedDistributedExecution}");

            _messageBusCts = _messageBusCts ?? new CancellationTokenSource();
            
            await _awsQueueClient.SendToQueue(
                _awsConfiguration.ScheduleRuleDistributedWorkQueueName,
                serialisedDistributedExecution,
                _messageBusCts.Token);
        }
    }
}
