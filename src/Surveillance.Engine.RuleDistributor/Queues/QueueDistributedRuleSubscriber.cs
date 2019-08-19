namespace Surveillance.Engine.RuleDistributor.Queues
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling.Interfaces;

    using Infrastructure.Network.Aws;
    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.RuleDistributor.Distributor.Interfaces;
    using Surveillance.Engine.RuleDistributor.Queues.Interfaces;

    public class QueueDistributedRuleSubscriber : IQueueDistributedRuleSubscriber
    {
        private readonly IAwsConfiguration _awsConfiguration;

        private readonly IAwsQueueClient _awsQueueClient;

        private readonly ILogger<QueueDistributedRuleSubscriber> _logger;

        private readonly IScheduledExecutionMessageBusSerialiser _messageBusSerialiser;

        private readonly IScheduleDisassembler _scheduleDisassembler;

        private readonly ISystemProcessContext _systemProcessContext;

        private CancellationTokenSource _messageBusCts;

        private AwsResusableCancellationToken _token;

        public QueueDistributedRuleSubscriber(
            IScheduleDisassembler scheduleDisassembler,
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IScheduledExecutionMessageBusSerialiser messageBusSerialiser,
            ISystemProcessContext systemProcessContext,
            ILogger<QueueDistributedRuleSubscriber> logger)
        {
            this._scheduleDisassembler =
                scheduleDisassembler ?? throw new ArgumentNullException(nameof(scheduleDisassembler));
            this._awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            this._awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            this._messageBusSerialiser =
                messageBusSerialiser ?? throw new ArgumentNullException(nameof(messageBusSerialiser));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this._systemProcessContext =
                systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));
        }

        public async Task ExecuteNonDistributedMessage(string messageId, string messageBody)
        {
            try
            {
                var opCtx = this._systemProcessContext.CreateAndStartOperationContext();

                this._logger.LogInformation(
                    $"read message {messageId} with body {messageBody} from {this._awsConfiguration.ScheduledRuleQueueName} for operation {opCtx.Id}");

                var execution = this._messageBusSerialiser.DeserialisedScheduledExecution(messageBody);

                if (execution == null)
                {
                    this._logger.LogError($"was unable to deserialise the message {messageId}");
                    opCtx.EndEventWithError($"was unable to deserialise the message {messageId}");
                    return;
                }

                await this._scheduleDisassembler.Disassemble(opCtx, execution, messageId, messageBody);
            }
            catch (Exception e)
            {
                this._logger.LogError(
                    $"execute non distributed message encountered a top level exception. {e.Message} {e.InnerException?.Message}",
                    e);
            }
        }

        public void Initiate()
        {
            this._logger.LogInformation("initiating");
            this._messageBusCts?.Cancel();
            this._messageBusCts = new CancellationTokenSource();
            this._token = new AwsResusableCancellationToken();

            this._awsQueueClient.SubscribeToQueueAsync(
                this._awsConfiguration.ScheduledRuleQueueName,
                async (s1, s2) => { await this.ExecuteNonDistributedMessage(s1, s2); },
                this._messageBusCts.Token,
                this._token);

            this._logger.LogInformation("completed initiating");
        }

        public void Terminate()
        {
            this._logger.LogInformation("sent terminate signal to cancellation token reading message bus");
            this._messageBusCts?.Cancel();
            this._messageBusCts = null;
        }
    }
}