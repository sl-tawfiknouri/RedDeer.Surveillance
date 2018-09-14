using System;
using System.Threading;
using System.Threading.Tasks;
using MessageBusDtos.Surveillance;
using MessageBusDtos.Surveillance.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.MessageBus_IO.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.MessageBus_IO
{
    public class CaseMessageSender : ICaseMessageSender
    {
        private readonly ICaseMessageBusSerialiser _serialiser;
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly ILogger<CaseMessageSender> _logger;

        public CaseMessageSender(
            ICaseMessageBusSerialiser serialiser,
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            ILogger<CaseMessageSender> logger)
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
                return;
            }

            var caseMessage = _serialiser.Serialise(message);
            var messageBusCts = new CancellationTokenSource();

            try
            {
                await _awsQueueClient.SendToQueue(_awsConfiguration.CaseMessageQueueName, caseMessage, messageBusCts.Token);
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception in Case Message Sender sending message '{caseMessage}' to bus on queue {_awsConfiguration.CaseMessageQueueName}. Error was {e.Message}");
            }
        }
    }
}
