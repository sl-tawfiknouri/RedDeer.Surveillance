using System;
using System.Threading;
using System.Threading.Tasks;
using Contracts.SurveillanceService;
using Contracts.SurveillanceService.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.MessageBusIO.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.MessageBusIO
{
    public class CaseMessageSender : ICaseMessageSender
    {
        private readonly IMessageBusSerialiser _serialiser;
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly ILogger<CaseMessageSender> _logger;

        public CaseMessageSender(
            IMessageBusSerialiser serialiser,
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
                _logger.LogWarning("CaseMessageSender tried to send a null case message. Did not send to AWS.");
                return;
            }

            var caseMessage = _serialiser.Serialise(message);
            var messageBusCts = new CancellationTokenSource();

            try
            {
                _logger.LogInformation($"CaseMessageSender Send | about to dispatch case {message.Case?.Title} to AWS queue | start of period {message.Case?.StartOfPeriodUnderInvestigation} | end of period {message.Case?.EndOfPeriodUnderInvestigation} | {message.CaseLogs?.Length} case log entries");
                await _awsQueueClient.SendToQueue(_awsConfiguration.CaseMessageQueueName, caseMessage, messageBusCts.Token);
                _logger.LogInformation($"CaseMessageSender Send | now dispatched case with title {message.Case?.Title}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception in Case Message Sender sending message '{caseMessage}' to bus on queue {_awsConfiguration.CaseMessageQueueName}. Error was {e.Message}");
            }
        }
    }
}
