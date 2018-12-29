using System;
using System.Threading;
using System.Threading.Tasks;
using DomainV2.DTO;
using DomainV2.DTO.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.MessageBusIO.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.MessageBusIO
{
    public class DataRequestMessageSender : IDataRequestMessageSender
    {
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly ILogger<DataRequestMessageSender> _logger;
        private readonly IThirdPartyDataRequestSerialiser _serialiser;

        public DataRequestMessageSender(
            IAwsConfiguration awsConfiguration,
            IAwsQueueClient awsQueueClient,
            IThirdPartyDataRequestSerialiser serialiser,
            ILogger<DataRequestMessageSender> logger)
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
                _logger.LogError($"DataRequestMessageSender received a null or empty rule run id");
                return;
            }

            var messageBusCts = new CancellationTokenSource();
            var message = new ThirdPartyDataRequestMessage {SystemProcessOperationRuleRunId = ruleRunId};
            var serialisedMessage = _serialiser.Serialise(message);

            try
            {
                await _awsQueueClient.SendToQueue(
                    _awsConfiguration.DataSynchroniserRequestQueueName,
                    serialisedMessage,
                    messageBusCts.Token);
            }
            catch (Exception e)
            {
                _logger.LogError($"DataRequestMessageSender encountered an error {e.Message} {e.InnerException?.Message} when sending rule run id {ruleRunId} to the data requests queue.");
            }
        }
    }
}
