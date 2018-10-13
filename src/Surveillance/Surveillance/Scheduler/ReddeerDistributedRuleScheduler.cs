using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Scheduling;
using Domain.Scheduling.Interfaces;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.Scheduler.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
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

            _awsQueueClient.SubscribeToQueueAsync(
                _awsConfiguration.ScheduledRuleQueueName,
                async (s1, s2) => { await ExecuteNonDistributedMessage(s1, s2); },
                _messageBusCts.Token);
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
                opCtx.EndEventWithError();
                return;
            }

            if (execution?.Rules == null
                || !execution.Rules.Any())
            {
                _logger.LogError($"ReddeerRuleScheduler deserialised message {messageId} but could not find any rules on the scheduled execution");
                opCtx.EndEventWithError();
                return;
            }

            var ruleTimespan = execution.TimeSeriesTermination - execution.TimeSeriesInitiation;
            var parameters =
                ruleTimespan.TotalDays >= 7
                ? await _ruleParameterApiRepository.Get()
                : null;

            var ruleCtx = 
                opCtx
                    .CreateAndStartDistributeRuleContext(
                        execution.TimeSeriesInitiation.DateTime,
                        execution.TimeSeriesTermination.DateTime,
                        execution.Rules?.Aggregate(string.Empty, (x,i) => x + ", " + i.GetDescription()).Trim(',', ' '));

            foreach (var rule in execution.Rules)
            {
                await ScheduleRule(rule, execution, parameters);
            }

            ruleCtx
                .EndEvent()
                .EndEvent();
        }

        private async Task ScheduleRule(Domain.Scheduling.Rules rule, ScheduledExecution execution, RuleParameterDto dtos)
        {
            var ruleTimespan = execution.TimeSeriesTermination - execution.TimeSeriesInitiation;
            var ruleParameterTimeWindow = RuleParameterTimespan(rule, dtos);

            if (ruleParameterTimeWindow == null
                || ruleTimespan.TotalDays < 7)
            {
                await ScheduleSingleExecution(rule, execution);
                return;
            }

            if (ruleParameterTimeWindow.GetValueOrDefault().TotalDays >= ruleTimespan.TotalDays)
            {
                await ScheduleSingleExecution(rule, execution);
                return;
            }

            var daysToRunRuleFor = Math.Max(6, ruleParameterTimeWindow.GetValueOrDefault().TotalDays * 3);

            if (daysToRunRuleFor >= ruleTimespan.TotalDays)
            {
                await ScheduleSingleExecution(rule, execution);
                return;
            }

            await TimeSplitter(rule, execution, ruleParameterTimeWindow, daysToRunRuleFor);
        }

        private async Task ScheduleSingleExecution(Domain.Scheduling.Rules rule, ScheduledExecution execution)
        {
            var distributedExecution = new ScheduledExecution
            {
                Rules = new List<Domain.Scheduling.Rules> { rule },
                TimeSeriesInitiation = execution.TimeSeriesInitiation,
                TimeSeriesTermination = execution.TimeSeriesTermination
            };

            await ScheduleExecution(distributedExecution);
        }

        private async Task TimeSplitter(
            Domain.Scheduling.Rules rule,
            ScheduledExecution execution,
            TimeSpan? ruleParameterTimeWindow,
            double daysToRunRuleFor)
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
                    Rules = new List<Domain.Scheduling.Rules> { rule },
                    TimeSeriesInitiation = currentInitiationPoint,
                    TimeSeriesTermination = currentEndPoint
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

        private TimeSpan? RuleParameterTimespan(Domain.Scheduling.Rules rule, RuleParameterDto dtos)
        {
            if (dtos == null)
            {
                return null;
            }

            switch (rule)
            {
                case Domain.Scheduling.Rules.CancelledOrders:
                    return dtos?.CancelledOrder?.WindowSize;
                case Domain.Scheduling.Rules.HighProfits:
                    return dtos?.HighProfits?.WindowSize;
                case Domain.Scheduling.Rules.MarkingTheClose:
                    return dtos?.MarkingTheClose?.Window;
                case Domain.Scheduling.Rules.Spoofing:
                    return dtos?.Spoofing?.WindowSize;
                case Domain.Scheduling.Rules.Layering:
                    return TimeSpan.FromMinutes(25);
                default:
                    return null;
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
