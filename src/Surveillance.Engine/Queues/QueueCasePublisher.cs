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
    public class QueueCasePublisher : IQueueCasePublisher
    {
        private readonly IMessageBusSerialiser _serialiser;
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly ILogger<QueueCasePublisher> _logger;

        public QueueCasePublisher(
            IMessageBusSerialiser serialiser,
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            ILogger<QueueCasePublisher> logger)
        {
            _serialiser = serialiser ?? throw new ArgumentNullException(nameof(serialiser));
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Send(CaseMessage message)
        {
            if (message == null)
            {
                _logger.LogWarning("QueueCasePublisher tried to send a null case message. Did not send to AWS.");
                return;
            }

            var caseMessage = _serialiser.Serialise(message);
            var messageBusCts = new CancellationTokenSource();

            try
            {
                _logger.LogInformation($"QueueCasePublisher Send | about to dispatch case {message.RuleBreachId} (id) to AWS queue");
                  await _awsQueueClient.SendToQueue(_awsConfiguration.CaseMessageQueueName, caseMessage, messageBusCts.Token);
                _logger.LogInformation($"QueueCasePublisher Send | now dispatched case with id {message.RuleBreachId}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception in QueueCasePublisher sending message '{caseMessage}' to bus on queue {_awsConfiguration.CaseMessageQueueName}. Error was {e.Message}");
            }
        }
    }
}
