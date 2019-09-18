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

    /// <summary>
    /// The schedule disassembler.
    /// </summary>
    public class ScheduleDisassembler : IScheduleDisassembler
    {
        /// <summary>
        /// The rule parameter repository.
        /// </summary>
        private readonly IRuleParameterApi ruleParameterApiRepository;

        /// <summary>
        /// The rule publisher.
        /// </summary>
        private readonly IQueueDistributedRulePublisher rulePublisher;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<ScheduleDisassembler> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleDisassembler"/> class.
        /// </summary>
        /// <param name="ruleParameterApiRepository">
        /// The rule parameter repository.
        /// </param>
        /// <param name="rulePublisher">
        /// The rule publisher.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ScheduleDisassembler(
            IRuleParameterApi ruleParameterApiRepository,
            IQueueDistributedRulePublisher rulePublisher,
            ILogger<ScheduleDisassembler> logger)
        {
            this.ruleParameterApiRepository =
                ruleParameterApiRepository ?? throw new ArgumentNullException(nameof(ruleParameterApiRepository));
            this.rulePublisher = rulePublisher ?? throw new ArgumentNullException(nameof(rulePublisher));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The disassemble.
        /// </summary>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="messageBody">
        /// The message body.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Disassemble(
            ISystemProcessOperationContext operationContext,
            ScheduledExecution execution,
            string messageId,
            string messageBody)
        {
            try
            {
                if (execution?.Rules == null || !execution.Rules.Any())
                {
                    this.logger.LogError(
                        $"deserialised message {messageId} but could not find any rules on the scheduled execution");
                    operationContext.EndEventWithError(
                        $"deserialised message {messageId} but could not find any rules on the scheduled execution");
                    return;
                }

                var scheduleRule = this.ValidateScheduleRule(execution);
                if (!scheduleRule)
                {
                    operationContext.EndEventWithError(
                        "did not validate the scheduled execution passed through. Check error logs.");
                    return;
                }

                var parameters = await this.ruleParameterApiRepository.Get();
                var ruleCtx = this.BuildRuleContext(operationContext, execution);
                await this.ScheduleRule(execution, parameters, ruleCtx);

                ruleCtx.EndEvent().EndEvent();

                this.logger.LogInformation(
                    $"read message {messageId} with body {messageBody} for operation {operationContext.Id} has completed");
            }
            catch (Exception e)
            {
                this.logger.LogError(
                    $"execute non distributed message encountered a top level exception. {e.Message} {e.InnerException?.Message}",
                    e);
            }
        }

        /// <summary>
        /// The validate schedule rule.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool ValidateScheduleRule(ScheduledExecution execution)
        {
            if (execution == null)
            {
                this.logger?.LogError("had a null scheduled execution. Returning.");
                return false;
            }

            if (execution.TimeSeriesInitiation.DateTime.Year < 2015)
            {
                this.logger?.LogError("had a time series initiation before 2015. Returning.");
                return false;
            }

            if (execution.TimeSeriesTermination.DateTime.Year < 2015)
            {
                this.logger?.LogError("had a time series termination before 2015. Returning.");
                return false;
            }

            if (execution.TimeSeriesInitiation > execution.TimeSeriesTermination)
            {
                this.logger?.LogError("had a time series initiation that exceeded the time series termination.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// The build rule context.
        /// </summary>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <returns>
        /// The <see cref="ISystemProcessOperationDistributeRuleContext"/>.
        /// </returns>
        private ISystemProcessOperationDistributeRuleContext BuildRuleContext(
            ISystemProcessOperationContext operationContext,
            ScheduledExecution execution)
        {
            var rules = 
                execution
                    .Rules
                    ?.Aggregate(
                        string.Empty,
                        (_, __) => _ + ", " + __.Rule.GetDescription() + $" ({(__.Ids.Any() ? __.Ids?.Aggregate((a, b) => a + "," + b)?.Trim(',') ?? string.Empty : string.Empty)})")
                .Trim(',', ' ');

            return operationContext.CreateAndStartDistributeRuleContext(
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime,
                rules);
        }

        /// <summary>
        /// The schedule rule.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="ruleContext">
        /// The rule context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task ScheduleRule(
            ScheduledExecution execution,
            RuleParameterDto parameters,
            ISystemProcessOperationDistributeRuleContext ruleContext)
        {
            foreach (var rule in execution.Rules.Where(ru => ru != null))
                switch (rule.Rule)
                {
                    case Rules.CancelledOrders:
                        var cancelledOrderRuleRuns =
                            parameters.CancelledOrders?.Select(co => co as IIdentifiableRule)?.ToList();
                        await this.ScheduleRuleRuns(execution, cancelledOrderRuleRuns, rule, ruleContext);
                        break;
                    case Rules.HighProfits:
                        var highProfitRuleRuns =
                            parameters.HighProfits?.Select(co => co as IIdentifiableRule)?.ToList();
                        await this.ScheduleRuleRuns(execution, highProfitRuleRuns, rule, ruleContext);
                        break;
                    case Rules.HighVolume:
                        var highVolumeRuleRuns =
                            parameters.HighVolumes?.Select(co => co as IIdentifiableRule)?.ToList();
                        await this.ScheduleRuleRuns(execution, highVolumeRuleRuns, rule, ruleContext);
                        break;
                    case Rules.MarkingTheClose:
                        var markingTheCloseRuleRuns =
                            parameters.MarkingTheCloses?.Select(co => co as IIdentifiableRule)?.ToList();
                        await this.ScheduleRuleRuns(execution, markingTheCloseRuleRuns, rule, ruleContext);
                        break;
                    case Rules.WashTrade:
                        var washTradeRuleRuns = parameters.WashTrades?.Select(co => co as IIdentifiableRule)?.ToList();
                        await this.ScheduleRuleRuns(execution, washTradeRuleRuns, rule, ruleContext);
                        break;
                    case Rules.UniverseFilter:
                        break;
                    case Rules.Spoofing:
                        var spoofingRuleRuns = parameters.Spoofings?.Select(co => co as IIdentifiableRule)?.ToList();
                        await this.ScheduleRuleRuns(execution, spoofingRuleRuns, rule, ruleContext);
                        break;
                    case Rules.PlacingOrderWithNoIntentToExecute:
                        var placingOrderRuleRuns =
                            parameters.PlacingOrders?.Select(_ => _ as IIdentifiableRule)?.ToList();
                        await this.ScheduleRuleRuns(execution, placingOrderRuleRuns, rule, ruleContext);
                        break;
                    case Rules.Layering:
                        // var layeringRuleRuns = parameters.Layerings?.Select(co => co as IIdentifiableRule)?.ToList();
                        // await ScheduleRuleRuns(execution, layeringRuleRuns, rule, ruleCtx);
                        break;
                    case Rules.FixedIncomeHighProfits:
                        var fixedIncomeHighProfits = parameters.FixedIncomeHighProfits
                            ?.Select(co => co as IIdentifiableRule)?.ToList();
                        await this.ScheduleRuleRuns(execution, fixedIncomeHighProfits, rule, ruleContext);
                        break;
                    case Rules.FixedIncomeHighVolumeIssuance:
                        var fixedIncomeHighVolumeIssuance = parameters.FixedIncomeHighVolumeIssuance
                            ?.Select(co => co as IIdentifiableRule)?.ToList();
                        await this.ScheduleRuleRuns(execution, fixedIncomeHighVolumeIssuance, rule, ruleContext);
                        break;
                    case Rules.FixedIncomeWashTrades:
                        var fixedIncomeWashTrade = parameters.FixedIncomeWashTrades
                            ?.Select(co => co as IIdentifiableRule)?.ToList();
                        await this.ScheduleRuleRuns(execution, fixedIncomeWashTrade, rule, ruleContext);
                        break;
                    case Rules.Ramping:
                        var ramping = parameters.Rampings?.Select(co => co as IIdentifiableRule)?.ToList();
                        await this.ScheduleRuleRuns(execution, ramping, rule, ruleContext);
                        break;
                    default:
                        this.logger.LogError(
                            $"{rule.Rule} was scheduled but not recognised by the Schedule Rule method in distributed rule.");
                        ruleContext.EventError(
                            $"{rule.Rule} was scheduled but not recognised by the Schedule Rule method in distributed rule.");
                        break;
                }
        }

        /// <summary>
        /// The schedule rule.
        /// </summary>
        /// <param name="ruleIdentifier">
        /// The rule identifier.
        /// </param>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="timespan">
        /// The timespan.
        /// </param>
        /// <param name="correlationId">
        /// The correlation id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
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
                this.logger.LogInformation("had a rule time span below 7 days. Scheduling single execution.");
                await this.ScheduleSingleExecution(ruleIdentifier, execution, correlationId);
                return;
            }

            if (ruleParameterTimeWindow.GetValueOrDefault().TotalDays >= ruleTimespan.TotalDays)
            {
                this.logger.LogInformation(
                    "had a rule parameter time window that exceeded the rule time span. Scheduling single execution.");
                await this.ScheduleSingleExecution(ruleIdentifier, execution, correlationId);
                return;
            }

            var daysToRunRuleFor = Math.Max(6, ruleParameterTimeWindow.GetValueOrDefault().TotalDays * 3);

            if (daysToRunRuleFor >= ruleTimespan.TotalDays)
            {
                this.logger.LogInformation(
                    $"had days to run rule for {daysToRunRuleFor} greater than or equal to rule time span total days {ruleTimespan.TotalDays} . Scheduling single execution.");

                await this.ScheduleSingleExecution(ruleIdentifier, execution, correlationId);
                return;
            }

            this.logger.LogInformation(
                "had a time span too excessive for the time window. Utilising time splitter to divide and conquer the requested rule run.");
            await this.TimeSplitter(
                ruleIdentifier,
                execution,
                ruleParameterTimeWindow,
                daysToRunRuleFor,
                correlationId);
        }

        /// <summary>
        /// The schedule rule runs.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="identifiableRules">
        /// The identifiable rules.
        /// </param>
        /// <param name="rule">
        /// The rule.
        /// </param>
        /// <param name="ruleContext">
        /// The rule context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task ScheduleRuleRuns(
            ScheduledExecution execution,
            IReadOnlyCollection<IIdentifiableRule> identifiableRules,
            RuleIdentifier rule,
            ISystemProcessOperationDistributeRuleContext ruleContext)
        {
            if (identifiableRules == null || !identifiableRules.Any())
            {
                this.logger.LogWarning($"did not have any identifiable rules for rule {rule.Rule}");
                return;
            }

            if (rule.Ids == null || !rule.Ids.Any())
            {
                foreach (var ruleSet in identifiableRules)
                {
                    var ruleInstance = new RuleIdentifier { Rule = rule.Rule, Ids = new[] { ruleSet.Id } };
                    await this.ScheduleRule(ruleInstance, execution, ruleSet.WindowSize, ruleContext.Id);
                }
            }
            else
            {
                foreach (var id in rule.Ids)
                {
                    var identifiableRule = 
                        identifiableRules?.FirstOrDefault(_ => 
                            string.Equals(_.Id, id, StringComparison.InvariantCultureIgnoreCase));

                    if (identifiableRule == null)
                    {
                        this.logger.LogError(
                            $"asked to schedule an execution for rule {rule.Rule.GetDescription()} with id of {id} which was not found when querying the rule parameter API on the client service.");

                        ruleContext.EventError(
                            $"asked to schedule an execution for rule {rule.Rule.GetDescription()} with id of {id} which was not found when querying the rule parameter API on the client service.");

                        continue;
                    }

                    var ruleInstance = new RuleIdentifier { Rule = rule.Rule, Ids = new[] { id } };

                    await this.ScheduleRule(ruleInstance, execution, identifiableRule.WindowSize, ruleContext.Id);
                }
            }
        }

        /// <summary>
        /// The schedule single execution.
        /// </summary>
        /// <param name="rule">
        /// The rule.
        /// </param>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="correlationId">
        /// The correlation id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
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

            await this.rulePublisher.ScheduleExecution(distributedExecution);
        }

        /// <summary>
        /// The time splitter.
        /// </summary>
        /// <param name="rule">
        /// The rule.
        /// </param>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="ruleParameterTimeWindow">
        /// The rule parameter time window.
        /// </param>
        /// <param name="daysToRunRuleFor">
        /// The days to run rule for.
        /// </param>
        /// <param name="correlationId">
        /// The correlation id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
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
                await this.rulePublisher.ScheduleExecution(executionSplit);
            }
        }
    }
}