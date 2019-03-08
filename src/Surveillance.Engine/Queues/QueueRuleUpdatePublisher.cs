using System;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Network.Aws.Interfaces;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService;
using RedDeer.Contracts.SurveillanceService.Interfaces;
using Surveillance.Engine.Rules.Queues.Interfaces;

namespace Surveillance.Engine.Rules.Queues
{
    public class QueueRuleUpdatePublisher : IQueueRuleUpdatePublisher
    {
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly ILogger<QueueRuleUpdatePublisher> _logger;
        private readonly IMessageBusSerialiser _serialiser;

        public QueueRuleUpdatePublisher(
            IAwsConfiguration awsConfiguration,
            IAwsQueueClient awsQueueClient,
            IMessageBusSerialiser serialiser,
            ILogger<QueueRuleUpdatePublisher> logger)
        {
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _serialiser = serialiser ?? throw new ArgumentNullException(nameof(serialiser));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Send(string ruleRunId)
        {
            if (string.IsNullOrWhiteSpace(ruleRunId))
            {
                _logger.LogError($"received a null or empty rule run id");
                return;
            }

            var messageBusCts = new CancellationTokenSource();

            var message = new TestRuleRunUpdateMessage
            {
                TestRuleRunId = ruleRunId
            };

            var serialisedMessage = _serialiser.Serialise(message);

            try
            {
                await _awsQueueClient.SendToQueue(
                    _awsConfiguration.TestRuleRunUpdateQueueName,
                    serialisedMessage,
                    messageBusCts.Token);
            }
            catch (Exception e)
            {
                _logger.LogError($"encountered an error {e.Message} {e.InnerException?.Message} when sending rule run id {ruleRunId} to rule run updates queue.", e);
            }
        }
    }
}
