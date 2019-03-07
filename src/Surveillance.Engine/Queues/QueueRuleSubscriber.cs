using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Surveillance.Scheduling;
using Domain.Surveillance.Scheduling.Interfaces;
using Infrastructure.Network.Aws;
using Infrastructure.Network.Aws.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Auditing.DataLayer.Processes;
using Surveillance.Engine.Rules.Analysis.Interfaces;
using Surveillance.Engine.Rules.Queues.Interfaces;
using Surveillance.Engine.Rules.Utility.Interfaces;

namespace Surveillance.Engine.Rules.Queues
{
    public class QueueRuleSubscriber : IQueueRuleSubscriber
    {
        private readonly IAnalysisEngine _analysisEngine;
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly IScheduledExecutionMessageBusSerialiser _messageBusSerialiser;
        private readonly IApiHeartbeat _apiHeartbeat;
        private readonly ISystemProcessContext _systemProcessContext;

        private readonly ILogger<QueueRuleSubscriber> _logger;
        private CancellationTokenSource _messageBusCts;
        private AwsResusableCancellationToken _token;

        public QueueRuleSubscriber(
            IAnalysisEngine analysisEngine,
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IScheduledExecutionMessageBusSerialiser messageBusSerialiser,
            IApiHeartbeat apiHeartbeat,
            ISystemProcessContext systemProcessContext,
            ILogger<QueueRuleSubscriber> logger)
        {
            _analysisEngine = analysisEngine ?? throw new ArgumentNullException(nameof(analysisEngine));
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            _messageBusSerialiser = messageBusSerialiser ?? throw new ArgumentNullException(nameof(messageBusSerialiser));
            _apiHeartbeat = apiHeartbeat ?? throw new ArgumentNullException(nameof(apiHeartbeat));
            _systemProcessContext = systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _messageBusCts?.Cancel();

            _messageBusCts = new CancellationTokenSource();
            _token = new AwsResusableCancellationToken();

            _awsQueueClient.SubscribeToQueueAsync(
                _awsConfiguration.ScheduleRuleDistributedWorkQueueName,
                async (s1, s2) => { await ExecuteDistributedMessage(s1, s2); },
                _messageBusCts.Token,
                _token);
        }

        public void Terminate()
        {
            _messageBusCts?.Cancel();
            _messageBusCts = null;
        }

        public async Task ExecuteDistributedMessage(string messageId, string messageBody)
        {
            var opCtx = _systemProcessContext.CreateAndStartOperationContext();

            try
            {
                _logger.LogInformation($"ReddeerRuleScheduler read message {messageId} with body {messageBody} from {_awsConfiguration.ScheduleRuleDistributedWorkQueueName}");

                await BlockOnApisDown(opCtx);

                var execution = _messageBusSerialiser.DeserialisedScheduledExecution(messageBody);

                if (execution == null)
                {
                    _logger.LogError($"ReddeerRuleScheduler was unable to deserialise the message {messageId}");
                    opCtx.EndEventWithError($"ReddeerRuleScheduler was unable to deserialise the message {messageId}");
                    return;
                }

                _logger.LogInformation($"ReddeerRuleScheduler about to execute message {messageBody}");

                var scheduleRuleValid = ValidateScheduleRule(execution);
                if (!scheduleRuleValid)
                {
                    opCtx.EndEventWithError("ReddeerRuleScheduler did not like the scheduled execution passed through. Check error logs.");
                    return;
                }

                await _analysisEngine.Execute(execution, opCtx);
            }
            catch (Exception e)
            {
                _logger.LogError($"ReddeerRuleScheduler caught exception in execute distributed message for {messageBody}", e);
                opCtx.EndEventWithError(e.Message);
            }
        }

        private async Task BlockOnApisDown(ISystemProcessOperationContext opCtx)
        {
            var servicesRunning = await _apiHeartbeat.HeartsBeating();

            if (!servicesRunning)
            {
                _logger.LogWarning("Reddeer Rule Scheduler asked to executed distributed message but was unable to reach api services");
                // set status here
                opCtx.UpdateEventState(OperationState.BlockedClientServiceDown);
                opCtx.EventError($"Reddeer Rule Scheduler asked to executed distributed message but was unable to reach api services");
            }

            int servicesDownMinutes = 0;
            var exitClientServiceBlock = servicesRunning;
            while (!exitClientServiceBlock)
            {
                _logger.LogInformation($"ReddeerRuleScheduler APIs down on heartbeat requests. Sleeping for 30 seconds.");

                Thread.Sleep(30 * 1000);
                var apiHeartBeat = _apiHeartbeat.HeartsBeating();
                exitClientServiceBlock = apiHeartBeat.Result;
                servicesDownMinutes += 1;

                if (servicesDownMinutes == 15)
                {
                    _logger.LogError("Reddeer Rule Scheduler has been trying to process a message for 15 minutes but the api services on the client service have been down");
                    opCtx.EventError($"Reddeer Rule Scheduler has been trying to process a message for 15 minutes but the api services on the client service have been down");
                }
            }

            if (!servicesRunning)
            {
                _logger.LogWarning("Reddeer Rule Scheduler was unable to reach api services but is now able to");
                opCtx.UpdateEventState(OperationState.InProcess);
            }
        }

        protected bool ValidateScheduleRule(ScheduledExecution execution)
        {
            if (execution == null)
            {
                _logger?.LogError($"ReddeerRuleScheduler had a null scheduled execution. Returning.");
                return false;
            }

            if (execution.TimeSeriesInitiation.DateTime.Year < 2015)
            {
                _logger?.LogError($"ReddeerRuleScheduler had a time series initiation before 2015. Returning.");
                return false;
            }

            if (execution.TimeSeriesTermination.DateTime.Year < 2015)
            {
                _logger?.LogError($"ReddeerRuleScheduler had a time series termination before 2015. Returning.");
                return false;
            }

            if (execution.TimeSeriesInitiation > execution.TimeSeriesTermination)
            {
                _logger?.LogError($"ReddeerRuleScheduler had a time series initiation that exceeded the time series termination.");
                return false;
            }

            return true;
        }
    }
}