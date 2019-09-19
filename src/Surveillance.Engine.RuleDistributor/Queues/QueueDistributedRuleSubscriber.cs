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

    /// <summary>
    /// The queue distributed rule subscriber.
    /// </summary>
    public class QueueDistributedRuleSubscriber : IQueueDistributedRuleSubscriber
    {
        /// <summary>
        /// The aws configuration.
        /// </summary>
        private readonly IAwsConfiguration awsConfiguration;

        /// <summary>
        /// The aws queue client.
        /// </summary>
        private readonly IAwsQueueClient awsQueueClient;

        /// <summary>
        /// The message bus serializer.
        /// </summary>
        private readonly IScheduledExecutionMessageBusSerialiser messageBusSerializer;

        /// <summary>
        /// The schedule disassembler.
        /// </summary>
        private readonly IScheduleDisassembler scheduleDisassembler;

        /// <summary>
        /// The system process context.
        /// </summary>
        private readonly ISystemProcessContext systemProcessContext;

        /// <summary>
        /// The message bus cancellation token source.
        /// </summary>
        private CancellationTokenSource messageBusCancellationTokenSource;
        
        /// <summary>
        /// The token.
        /// </summary>
        private AwsResusableCancellationToken token;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<QueueDistributedRuleSubscriber> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueDistributedRuleSubscriber"/> class.
        /// </summary>
        /// <param name="scheduleDisassembler">
        /// The schedule disassembler.
        /// </param>
        /// <param name="awsQueueClient">
        /// The aws queue client.
        /// </param>
        /// <param name="awsConfiguration">
        /// The aws configuration.
        /// </param>
        /// <param name="messageBusSerialiser">
        /// The message bus serializer.
        /// </param>
        /// <param name="systemProcessContext">
        /// The system process context.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public QueueDistributedRuleSubscriber(
            IScheduleDisassembler scheduleDisassembler,
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IScheduledExecutionMessageBusSerialiser messageBusSerialiser,
            ISystemProcessContext systemProcessContext,
            ILogger<QueueDistributedRuleSubscriber> logger)
        {
            this.scheduleDisassembler =
                scheduleDisassembler ?? throw new ArgumentNullException(nameof(scheduleDisassembler));
            this.awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            this.awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            this.messageBusSerializer =
                messageBusSerialiser ?? throw new ArgumentNullException(nameof(messageBusSerialiser));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this.systemProcessContext =
                systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));
        }

        /// <summary>
        /// The execute non distributed message.
        /// </summary>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="messageBody">
        /// The message body.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task ExecuteNonDistributedMessage(string messageId, string messageBody)
        {
            try
            {
                var operationContext = this.systemProcessContext.CreateAndStartOperationContext();

                this.logger.LogInformation(
                    $"read message {messageId} with body {messageBody} from {this.awsConfiguration.ScheduledRuleQueueName} for operation {operationContext.Id}");

                var execution = this.messageBusSerializer.DeserialisedScheduledExecution(messageBody);

                if (execution == null)
                {
                    this.logger.LogError($"was unable to deserialise the message {messageId}");
                    operationContext.EndEventWithError($"was unable to deserialise the message {messageId}");

                    return;
                }

                await this.scheduleDisassembler.Disassemble(operationContext, execution, messageId, messageBody).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                this.logger.LogError(
                    $"execute non distributed message encountered a top level exception. {e.Message} {e.InnerException?.Message}",
                    e);
            }
        }

        /// <summary>
        /// The initiate.
        /// </summary>
        public void Initiate()
        {
            this.logger.LogInformation("initiating");
            this.messageBusCancellationTokenSource?.Cancel();
            this.messageBusCancellationTokenSource = new CancellationTokenSource();
            this.token = new AwsResusableCancellationToken();

            this.awsQueueClient.SubscribeToQueueAsync(
                this.awsConfiguration.ScheduledRuleQueueName,
                async (s1, s2) => { await this.ExecuteNonDistributedMessage(s1, s2).ConfigureAwait(false); },
                this.messageBusCancellationTokenSource.Token,
                this.token);

            this.logger.LogInformation("completed initiating");
        }

        /// <summary>
        /// The terminate.
        /// </summary>
        public void Terminate()
        {
            this.logger.LogInformation("sent terminate signal to cancellation token reading message bus");
            this.messageBusCancellationTokenSource?.Cancel();
            this.messageBusCancellationTokenSource = null;
        }
    }
}