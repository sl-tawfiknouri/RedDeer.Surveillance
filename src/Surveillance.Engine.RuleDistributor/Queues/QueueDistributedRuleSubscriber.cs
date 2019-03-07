using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Surveillance.Scheduling.Interfaces;
using Infrastructure.Network.Aws_IO;
using Infrastructure.Network.Aws_IO.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.RuleDistributor.Distributor.Interfaces;
using Surveillance.Engine.RuleDistributor.Queues.Interfaces;

namespace Surveillance.Engine.RuleDistributor.Queues
{
    public class QueueDistributedRuleSubscriber : IQueueDistributedRuleSubscriber
    {
        private readonly IScheduleDisassembler _scheduleDisassembler;
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly IScheduledExecutionMessageBusSerialiser _messageBusSerialiser;
        private readonly ISystemProcessContext _systemProcessContext;

        private readonly ILogger<QueueDistributedRuleSubscriber> _logger;
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
            _scheduleDisassembler = scheduleDisassembler ?? throw new ArgumentNullException(nameof(scheduleDisassembler));
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            _messageBusSerialiser = messageBusSerialiser ?? throw new ArgumentNullException(nameof(messageBusSerialiser));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _systemProcessContext =
                systemProcessContext
                ?? throw new ArgumentNullException(nameof(systemProcessContext));
        }

        public void Initiate()
        {
            _logger.LogInformation($"ReddeerDistributedRuleScheduler initiating");
            _messageBusCts?.Cancel();
            _messageBusCts = new CancellationTokenSource();
            _token = new AwsResusableCancellationToken();

            _awsQueueClient.SubscribeToQueueAsync(
                _awsConfiguration.ScheduledRuleQueueName,
                async (s1, s2) => { await ExecuteNonDistributedMessage(s1, s2); },
                _messageBusCts.Token,
                _token);

            _logger.LogInformation($"ReddeerDistributedRuleScheduler completed initiating");
        }

        public void Terminate()
        {
            _logger.LogInformation($"ReddeerDistributedRuleScheduler sent terminate signal to cancellation token reading message bus");
            _messageBusCts?.Cancel();
            _messageBusCts = null;
        }

        public async Task ExecuteNonDistributedMessage(string messageId, string messageBody)
        {
            try
            {
                var opCtx = _systemProcessContext.CreateAndStartOperationContext();

                _logger.LogInformation($"ReddeerDistributedRuleScheduler read message {messageId} with body {messageBody} from {_awsConfiguration.ScheduledRuleQueueName} for operation {opCtx.Id}");

                var execution = _messageBusSerialiser.DeserialisedScheduledExecution(messageBody);

                if (execution == null)
                {
                    _logger.LogError($"ReddeerDistributedRuleScheduler was unable to deserialise the message {messageId}");
                    opCtx.EndEventWithError($"ReddeerDistributedRuleScheduler was unable to deserialise the message {messageId}");
                    return;
                }

                await _scheduleDisassembler.Disassemble(opCtx, execution, messageId, messageBody);
            }
            catch (Exception e)
            {
                _logger.LogError($"ReddeerDistributedRuleScheduler execute non distributed message encountered a top level exception.", e);
            }
        }
    }
}
