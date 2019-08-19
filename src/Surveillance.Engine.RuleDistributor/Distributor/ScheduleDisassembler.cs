namespace Surveillance.Engine.RuleDistributor.Distributor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Domain.Core.Extensions;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Interfaces;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.RuleDistributor.Distributor.Interfaces;
    using Surveillance.Engine.RuleDistributor.Queues.Interfaces;
    using Surveillance.Reddeer.ApiClient.RuleParameter.Interfaces;

    public class ScheduleDisassembler : IScheduleDisassembler
    {
        private readonly ILogger<ScheduleDisassembler> _logger;

        private readonly IRuleParameterApi _ruleParameterApiRepository;

        private readonly IQueueDistributedRulePublisher _rulePublisher;

        public ScheduleDisassembler(
            IRuleParameterApi ruleParameterApiRepository,
            IQueueDistributedRulePublisher rulePublisher,
            ILogger<ScheduleDisassembler> logger)
        {
            this._ruleParameterApiRepository = ruleParameterApiRepository
                                               ?? throw new ArgumentNullException(nameof(ruleParameterApiRepository));
            this._rulePublisher = rulePublisher ?? throw new ArgumentNullException(nameof(rulePublisher));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Disassemble(
            ISystemProcessOperationContext opCtx,
            ScheduledExecution execution,
            string messageId,
            string messageBody)
        {
            try
            {
                if (execution?.Rules == null || !execution.Rules.Any())
                {
                    this._logger.LogError(
                        $"deserialised message {messageId} but could not find any rules on the scheduled execution");
                    opCtx.EndEventWithError(
                        $"deserialised message {messageId} but could not find any rules on the scheduled execution");
                    return;
                }

                var scheduleRule = this.ValidateScheduleRule(execution);
                if (!scheduleRule)
                {
                    opCtx.EndEventWithError(
                        "did not validate the scheduled execution passed through. Check error logs.");
                    return;
                }

                var parameters = await this._ruleParameterApiRepository.Get();
                var ruleCtx = this.BuildRuleCtx(opCtx, execution);
                await this.ScheduleRule(execution, parameters, ruleCtx);

                ruleCtx.EndEvent().EndEvent();

                this._logger.LogInformation(
                    $"read message {messageId} with body {messageBody} for operation {opCtx.Id} has completed");
            }
            catch (Exception e)
            {
                this._logger.LogError(
                    $"execute non distributed message encountered a top level exception. {e.Message} {e.InnerException?.Message}",
                    e);
            }
        }

        protected bool ValidateScheduleRule(ScheduledExecution execution)
        {
            if (execution == null)
            {
                this._logger?.LogError("had a null scheduled execution. Returning.");
                return false;
            }

            if (execution.TimeSeriesInitiation.DateTime.Year < 2015)
            {
                this._logger?.LogError("had a time series initiation before 2015. Returning.");
                return false;
            }

            if (execution.TimeSeriesTermination.DateTime.Year < 2015)
            {
                this._logger?.LogError("had a time series termination before 2015. Returning.");
                return false;
            }

            if (execution.TimeSeriesInitiation > execution.TimeSeriesTermination)
            {
                this._logger?.LogError("had a time series initiation that exceeded the time series termination.");
                return false;
            }

            return true;
        }

        private ISystemProcessOperationDistributeRuleContext BuildRuleCtx(
            ISystemProcessOperationContext opCtx,
            ScheduledExecution execution)
        {
            return opCtx.CreateAndStartDistributeRuleContext(
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime,
                execution.Rules?.Aggregate(
                        string.Empty,
                        (x, i) => x + ", " + i.Rule.GetDescription()
                                  + $" ({(i.Ids.Any() ? i.Ids?.Aggregate((a, b) => a + "," + b)?.Trim(',') ?? string.Empty : string.Empty)})")
                    .Trim(',', ' '));
        }

        private async Task ScheduleRule(
            ScheduledExecution execution,
            RuleParameterDto parameters,
            ISystemProcessOperationDistributeRuleContext ruleCtx)
        {
            foreach (var rule in execution.Rules.Where(ru => ru != null))
                switch (rule.Rule)
                {
                    case Rules.CancelledOrders:
                        var cancelledOrderRuleRuns =
                            parameters.CancelledOrders?.Select(co => co as IIdentifiableRule)?.ToList();
                        await this.ScheduleRuleRuns(execution, cancelledOrderRuleRuns, rule, ruleCtx);
                        break;
                    case Rules.HighProfits:
                        var highProfitRuleRuns =
                            parameters.HighProfits?.Select(co => co as IIdentifiableRule)?.ToList();
                        await this.ScheduleRuleRuns(execution, highProfitRuleRuns, rule, ruleCtx);
                        break;
                    case Rules.HighVolume:
                        var highVolumeRuleRuns =
                            parameters.HighVolumes?.Select(co => co as IIdentifiableRule)?.ToList();
                        await this.ScheduleRuleRuns(execution, highVolumeRuleRuns, rule, ruleCtx);
                        break;
                    case Rules.MarkingTheClose:
                        var markingTheCloseRuleRuns =
                            parameters.MarkingTheCloses?.Select(co => co as IIdentifiableRule)?.ToList();
                        await this.ScheduleRuleRuns(execution, markingTheCloseRuleRuns, rule, ruleCtx);
                        break;
                    case Rules.WashTrade:
                        var washTradeRuleRuns = parameters.WashTrades?.Select(co => co as IIdentifiableRule)?.ToList();
                        await this.ScheduleRuleRuns(execution, washTradeRuleRuns, rule, ruleCtx);
                        break;
                    case Rules.UniverseFilter:
                        break;
                    case Rules.Spoofing:
                        var spoofingRuleRuns = parameters.Spoofings?.Select(co => co as IIdentifiableRule)?.ToList();
                        await this.ScheduleRuleRuns(execution, spoofingRuleRuns, rule, ruleCtx);
                        break;
                    case Rules.PlacingOrderWithNoIntentToExecute:
                        var placingOrderRuleRuns =
                            parameters.PlacingOrders?.Select(_ => _ as IIdentifiableRule)?.ToList();
                        await this.ScheduleRuleRuns(execution, placingOrderRuleRuns, rule, ruleCtx);
                        break;
                    case Rules.Layering:
                        // var layeringRuleRuns = parameters.Layerings?.Select(co => co as IIdentifiableRule)?.ToList();
                        // await ScheduleRuleRuns(execution, layeringRuleRuns, rule, ruleCtx);
                        break;
                    case Rules.FixedIncomeHighProfits:
                        var fixedIncomeHighProfits = parameters.FixedIncomeHighProfits
                            ?.Select(co => co as IIdentifiableRule)?.ToList();
                        await this.ScheduleRuleRuns(execution, fixedIncomeHighProfits, rule, ruleCtx);
                        break;
                    case Rules.FixedIncomeHighVolumeIssuance:
                        var fixedIncomeHighVolumeIssuance = parameters.FixedIncomeHighVolumeIssuance
                            ?.Select(co => co as IIdentifiableRule)?.ToList();
                        await this.ScheduleRuleRuns(execution, fixedIncomeHighVolumeIssuance, rule, ruleCtx);
                        break;
                    case Rules.FixedIncomeWashTrades:
                        var fixedIncomeWashTrade = parameters.FixedIncomeWashTrades
                            ?.Select(co => co as IIdentifiableRule)?.ToList();
                        await this.ScheduleRuleRuns(execution, fixedIncomeWashTrade, rule, ruleCtx);
                        break;
                    case Rules.Ramping:
                        var ramping = parameters.Rampings?.Select(co => co as IIdentifiableRule)?.ToList();
                        await this.ScheduleRuleRuns(execution, ramping, rule, ruleCtx);
                        break;
                    default:
                        this._logger.LogError(
                            $"{rule.Rule} was scheduled but not recognised by the Schedule Rule method in distributed rule.");
                        ruleCtx.EventError(
                            $"{rule.Rule} was scheduled but not recognised by the Schedule Rule method in distributed rule.");
                        break;
                }
        }

        private async Task ScheduleRule(
            RuleIdentifier ruleIdentifier,
            ScheduledExecution execution,
            TimeSpan? timespan,
            string correlationId)
        {
            var ruleTimespan = execution.TimeSeriesTermination - execution.TimeSeriesInitiation;
            var ruleParameterTimeWindow = timespan;

            if (ruleParameterTimeWindow == null || ruleTimespan.TotalDays < 7)
            {
                this._logger.LogInformation("had a rule time span below 7 days. Scheduling single execution.");
                await this.ScheduleSingleExecution(ruleIdentifier, execution, correlationId);
                return;
            }

            if (ruleParameterTimeWindow.GetValueOrDefault().TotalDays >= ruleTimespan.TotalDays)
            {
                this._logger.LogInformation(
                    "had a rule parameter time window that exceeded the rule time span. Scheduling single execution.");
                await this.ScheduleSingleExecution(ruleIdentifier, execution, correlationId);
                return;
            }

            var daysToRunRuleFor = Math.Max(6, ruleParameterTimeWindow.GetValueOrDefault().TotalDays * 3);

            if (daysToRunRuleFor >= ruleTimespan.TotalDays)
            {
                this._logger.LogInformation(
                    $"had days to run rule for {daysToRunRuleFor} greater than or equal to rule time span total days {ruleTimespan.TotalDays} . Scheduling single execution.");

                await this.ScheduleSingleExecution(ruleIdentifier, execution, correlationId);
                return;
            }

            this._logger.LogInformation(
                "had a time span too excessive for the time window. Utilising time splitter to divide and conquer the requested rule run.");
            await this.TimeSplitter(
                ruleIdentifier,
                execution,
                ruleParameterTimeWindow,
                daysToRunRuleFor,
                correlationId);
        }

        private async Task ScheduleRuleRuns(
            ScheduledExecution execution,
            IReadOnlyCollection<IIdentifiableRule> identifiableRules,
            RuleIdentifier rule,
            ISystemProcessOperationDistributeRuleContext ruleCtx)
        {
            if (identifiableRules == null || !identifiableRules.Any())
            {
                this._logger.LogWarning($"did not have any identifiable rules for rule {rule.Rule}");
                return;
            }

            if (rule.Ids == null || !rule.Ids.Any())
                foreach (var ruleSet in identifiableRules)
                {
                    var ruleInstance = new RuleIdentifier { Rule = rule.Rule, Ids = new[] { ruleSet.Id } };
                    await this.ScheduleRule(ruleInstance, execution, ruleSet.WindowSize, ruleCtx.Id);
                }
            else
                foreach (var id in rule.Ids)
                {
                    var identifiableRule = identifiableRules?.FirstOrDefault(
                        param => string.Equals(param.Id, id, StringComparison.InvariantCultureIgnoreCase));

                    if (identifiableRule == null)
                    {
                        this._logger.LogError(
                            $"asked to schedule an execution for rule {rule.Rule.GetDescription()} with id of {id} which was not found when querying the rule parameter API on the client service.");

                        ruleCtx.EventError(
                            $"asked to schedule an execution for rule {rule.Rule.GetDescription()} with id of {id} which was not found when querying the rule parameter API on the client service.");

                        continue;
                    }

                    var ruleInstance = new RuleIdentifier { Rule = rule.Rule, Ids = new[] { id } };
                    await this.ScheduleRule(ruleInstance, execution, identifiableRule.WindowSize, ruleCtx.Id);
                }
        }

        private async Task ScheduleSingleExecution(
            RuleIdentifier rule,
            ScheduledExecution execution,
            string correlationId)
        {
            var distributedExecution = new ScheduledExecution
                                           {
                                               Rules = new List<RuleIdentifier> { rule },
                                               TimeSeriesInitiation = execution.TimeSeriesInitiation,
                                               TimeSeriesTermination = execution.TimeSeriesTermination,
                                               CorrelationId = correlationId,
                                               IsBackTest = execution.IsBackTest
                                           };

            await this._rulePublisher.ScheduleExecution(distributedExecution);
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
                    currentEndPoint = execution.TimeSeriesTermination;

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

                if (currentInitiationPoint > execution.TimeSeriesTermination) continueSplit = false;

                currentInitiationPoint =
                    currentInitiationPoint.AddDays(-ruleParameterTimeWindow.GetValueOrDefault().TotalDays);
            }

            foreach (var executionSplit in executions) await this._rulePublisher.ScheduleExecution(executionSplit);
        }
    }
}