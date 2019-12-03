namespace Surveillance.Engine.Rules.Queues
{
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

    public class QueueRuleSubscriber : IQueueRuleSubscriber
    {
        private readonly IAnalysisEngine _analysisEngine;

        private readonly IApiHeartbeat _apiHeartbeat;

        private readonly IAwsConfiguration _awsConfiguration;

        private readonly IAwsQueueClient _awsQueueClient;

        private readonly ILogger<QueueRuleSubscriber> _logger;

        private readonly IScheduledExecutionMessageBusSerialiser _messageBusSerialiser;

        private readonly ISystemProcessContext _systemProcessContext;

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
            this._analysisEngine = analysisEngine ?? throw new ArgumentNullException(nameof(analysisEngine));
            this._awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            this._awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            this._messageBusSerialiser =
                messageBusSerialiser ?? throw new ArgumentNullException(nameof(messageBusSerialiser));
            this._apiHeartbeat = apiHeartbeat ?? throw new ArgumentNullException(nameof(apiHeartbeat));
            this._systemProcessContext =
                systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ExecuteDistributedMessage(string messageId, string messageBody)
        {
            var opCtx = this._systemProcessContext.CreateAndStartOperationContext();

            try
            {
                this._logger.LogInformation(
                    $"read message {messageId} with body {messageBody} from {this._awsConfiguration.ScheduleRuleDistributedWorkQueueName}");

                await this.BlockOnApisDown(opCtx);

                var execution = this._messageBusSerialiser.DeserialisedScheduledExecution(messageBody);

                if (execution == null)
                {
                    this._logger.LogError($"was unable to deserialise the message {messageId}");
                    opCtx.EndEventWithError($"was unable to deserialise the message {messageId}");
                    return;
                }

                this._logger.LogInformation($"about to execute message {messageBody}");

                var scheduleRuleValid = this.ValidateScheduleRule(execution);
                if (!scheduleRuleValid)
                {
                    opCtx.EndEventWithError("did not like the scheduled execution passed through. Check error logs.");
                    return;
                }

                await this._analysisEngine.Execute(execution, opCtx);
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"caught exception in execute distributed message for {messageBody}");
                opCtx.EndEventWithError(e.Message);
            }
        }

        public void Initiate()
        {
            this._messageBusCts?.Cancel();

            this._messageBusCts = new CancellationTokenSource();
            this._token = new AwsResusableCancellationToken();

            this._awsQueueClient.SubscribeToQueueAsync(
                this._awsConfiguration.ScheduleRuleDistributedWorkQueueName,
                async (s1, s2) => { await this.ExecuteDistributedMessage(s1, s2); },
                this._messageBusCts.Token,
                this._token);
        }

        public void Terminate()
        {
            this._messageBusCts?.Cancel();
            this._messageBusCts = null;
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

        private async Task BlockOnApisDown(ISystemProcessOperationContext opCtx)
        {
            var servicesRunning = await this._apiHeartbeat.HeartsBeating();

            if (!servicesRunning)
            {
                this._logger.LogWarning("asked to executed distributed message but was unable to reach api services");

                // set status here
                opCtx.UpdateEventState(OperationState.BlockedClientServiceDown);
                opCtx.EventError("asked to executed distributed message but was unable to reach api services");
            }

            var servicesDownMinutes = 0;
            var exitClientServiceBlock = servicesRunning;
            while (!exitClientServiceBlock)
            {
                this._logger.LogInformation("APIs down on heartbeat requests. Sleeping for 30 seconds.");

                Thread.Sleep(30 * 1000);
                var apiHeartBeat = this._apiHeartbeat.HeartsBeating();
                exitClientServiceBlock = apiHeartBeat.Result;
                servicesDownMinutes += 1;

                if (servicesDownMinutes == 15)
                {
                    this._logger.LogError(
                        "has been trying to process a message for 15 minutes but the api services on the client service have been down");
                    opCtx.EventError(
                        "has been trying to process a message for 15 minutes but the api services on the client service have been down");
                }
            }

            if (!servicesRunning)
            {
                this._logger.LogWarning("was unable to reach api services but is now able to");
                opCtx.UpdateEventState(OperationState.InProcess);
            }
        }
    }
}