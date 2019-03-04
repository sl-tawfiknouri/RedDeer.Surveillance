using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Core.Extensions;
using Domain.Scheduling;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Interfaces;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.Engine.RuleDistributor.Distributor.Interfaces;
using Surveillance.Engine.RuleDistributor.Queues.Interfaces;

namespace Surveillance.Engine.RuleDistributor.Distributor
{
    public class ScheduleDisassembler : IScheduleDisassembler
    {
        private readonly IRuleParameterApiRepository _ruleParameterApiRepository;
        private readonly IQueueDistributedRulePublisher _rulePublisher;
        private readonly ILogger<ScheduleDisassembler> _logger;
        
        public ScheduleDisassembler(
            IRuleParameterApiRepository ruleParameterApiRepository,
            IQueueDistributedRulePublisher rulePublisher,
            ILogger<ScheduleDisassembler> logger)
        {
            _ruleParameterApiRepository = ruleParameterApiRepository ?? throw new ArgumentNullException(nameof(ruleParameterApiRepository));
            _rulePublisher = rulePublisher ?? throw new ArgumentNullException(nameof(rulePublisher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Disassemble(
            ISystemProcessOperationContext opCtx,
            ScheduledExecution execution,
            string messageId,
            string messageBody)
        {
            try
            {
                if (execution?.Rules == null
                    || !execution.Rules.Any())
                {
                    _logger.LogError($"{nameof(ScheduleDisassembler)} deserialised message {messageId} but could not find any rules on the scheduled execution");
                    opCtx.EndEventWithError($"{nameof(ScheduleDisassembler)} deserialised message {messageId} but could not find any rules on the scheduled execution");
                    return;
                }

                var scheduleRule = ValidateScheduleRule(execution);
                if (!scheduleRule)
                {
                    opCtx.EndEventWithError($"{nameof(ScheduleDisassembler)} did not validate the scheduled execution passed through. Check error logs.");
                    return;
                }

                var parameters = await _ruleParameterApiRepository.Get();
                var ruleCtx = BuildRuleCtx(opCtx, execution);
                await ScheduleRule(execution, parameters, ruleCtx);

                ruleCtx
                    .EndEvent()
                    .EndEvent();

                _logger.LogInformation($"{nameof(ScheduleDisassembler)} read message {messageId} with body {messageBody} for operation {opCtx.Id} has completed");
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(ScheduleDisassembler)} execute non distributed message encountered a top level exception.", e);
            }
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
                    case Rules.CancelledOrders:
                        var cancelledOrderRuleRuns = parameters.CancelledOrders?.Select(co => co as IIdentifiableRule)?.ToList();
                        await ScheduleRuleRuns(execution, cancelledOrderRuleRuns, rule, ruleCtx);
                        break;
                    case Rules.HighProfits:
                        var highProfitRuleRuns = parameters.HighProfits?.Select(co => co as IIdentifiableRule)?.ToList();
                        await ScheduleRuleRuns(execution, highProfitRuleRuns, rule, ruleCtx);
                        break;
                    case Rules.HighVolume:
                        var highVolumeRuleRuns = parameters.HighVolumes?.Select(co => co as IIdentifiableRule)?.ToList();
                        await ScheduleRuleRuns(execution, highVolumeRuleRuns, rule, ruleCtx);
                        break;
                    case Rules.MarkingTheClose:
                        var markingTheCloseRuleRuns = parameters.MarkingTheCloses?.Select(co => co as IIdentifiableRule)?.ToList();
                        await ScheduleRuleRuns(execution, markingTheCloseRuleRuns, rule, ruleCtx);
                        break;
                    case Rules.WashTrade:
                        var washTradeRuleRuns = parameters.WashTrades?.Select(co => co as IIdentifiableRule)?.ToList();
                        await ScheduleRuleRuns(execution, washTradeRuleRuns, rule, ruleCtx);
                        break;
                    case Rules.UniverseFilter:
                        break;
                    case Rules.Spoofing:
                        // var spoofingRuleRuns = parameters.Spoofings?.Select(co => co as IIdentifiableRule)?.ToList();
                        // await ScheduleRuleRuns(execution, spoofingRuleRuns, rule, ruleCtx);
                        break;
                    case Rules.Layering:
                        // var layeringRuleRuns = parameters.Layerings?.Select(co => co as IIdentifiableRule)?.ToList();
                        // await ScheduleRuleRuns(execution, layeringRuleRuns, rule, ruleCtx);
                        break;
                    case Rules.FixedIncomeHighProfits:
                        var fixedIncomeHighProfits = parameters.FixedIncomeHighProfits?.Select(co => co as IIdentifiableRule)?.ToList();
                        await ScheduleRuleRuns(execution, fixedIncomeHighProfits, rule, ruleCtx);
                        break;
                    case Rules.FixedIncomeHighVolumeIssuance:
                        var fixedIncomeHighVolumeIssuance = parameters.FixedIncomeHighVolumeIssuance?.Select(co => co as IIdentifiableRule)?.ToList();
                        await ScheduleRuleRuns(execution, fixedIncomeHighVolumeIssuance, rule, ruleCtx);
                        break;
                    case Rules.FixedIncomeWashTrades:
                        var fixedIncomeWashTrade = parameters.FixedIncomeWashTrades?.Select(co => co as IIdentifiableRule)?.ToList();
                        await ScheduleRuleRuns(execution, fixedIncomeWashTrade, rule, ruleCtx);
                        break;
                    default:
                        _logger.LogError($"{nameof(ScheduleDisassembler)} {rule.Rule} was scheduled but not recognised by the Schedule Rule method in distributed rule.");
                        ruleCtx.EventError($"{nameof(ScheduleDisassembler)} {rule.Rule} was scheduled but not recognised by the Schedule Rule method in distributed rule.");
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
                _logger.LogWarning($"{nameof(ScheduleDisassembler)} did not have any identifiable rules for rule {rule.Rule}");
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
                        _logger.LogError($"{nameof(ScheduleDisassembler)} asked to schedule an execution for rule {rule.Rule.GetDescription()} with id of {id} which was not found when querying the rule parameter API on the client service.");

                        ruleCtx.EventError($"{nameof(ScheduleDisassembler)} asked to schedule an execution for rule {rule.Rule.GetDescription()} with id of {id} which was not found when querying the rule parameter API on the client service.");

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
                _logger.LogInformation($"{nameof(ScheduleDisassembler)} had a rule time span below 7 days. Scheduling single execution.");
                await ScheduleSingleExecution(ruleIdentifier, execution, correlationId);
                return;
            }

            if (ruleParameterTimeWindow.GetValueOrDefault().TotalDays >= ruleTimespan.TotalDays)
            {
                _logger.LogInformation($"{nameof(ScheduleDisassembler)} had a rule parameter time window that exceeded the rule time span. Scheduling single execution.");
                await ScheduleSingleExecution(ruleIdentifier, execution, correlationId);
                return;
            }

            var daysToRunRuleFor = Math.Max(6, ruleParameterTimeWindow.GetValueOrDefault().TotalDays * 3);

            if (daysToRunRuleFor >= ruleTimespan.TotalDays)
            {
                _logger.LogInformation($"{nameof(ScheduleDisassembler)} had days to run rule for {daysToRunRuleFor} greater than or equal to rule time span total days {ruleTimespan.TotalDays} . Scheduling single execution.");

                await ScheduleSingleExecution(ruleIdentifier, execution, correlationId);
                return;
            }

            _logger.LogInformation($"{nameof(ScheduleDisassembler)} had a time span too excessive for the time window. Utilising time splitter to divide and conquer the requested rule run.");
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

            await _rulePublisher.ScheduleExecution(distributedExecution);
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
                await _rulePublisher.ScheduleExecution(executionSplit);
            }
        }

        protected bool ValidateScheduleRule(ScheduledExecution execution)
        {
            if (execution == null)
            {
                _logger?.LogError($"{nameof(ScheduleDisassembler)} had a null scheduled execution. Returning.");
                return false;
            }

            if (execution.TimeSeriesInitiation.DateTime.Year < 2015)
            {
                _logger?.LogError($"{nameof(ScheduleDisassembler)} had a time series initiation before 2015. Returning.");
                return false;
            }

            if (execution.TimeSeriesTermination.DateTime.Year < 2015)
            {
                _logger?.LogError($"{nameof(ScheduleDisassembler)} had a time series termination before 2015. Returning.");
                return false;
            }

            if (execution.TimeSeriesInitiation > execution.TimeSeriesTermination)
            {
                _logger?.LogError($"{nameof(ScheduleDisassembler)} had a time series initiation that exceeded the time series termination.");
                return false;
            }

            return true;
        }
    }
}
