using System;
using System.Threading;
using System.Threading.Tasks;
using Contracts.SurveillanceService.Interfaces;
using DomainV2.Contracts;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.DataCoordinator.Queues.Interfaces;
using Utilities.Aws_IO;
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.Engine.DataCoordinator.Queues
{
    public class QueueSubscriber : IQueueSubscriber
    {
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly IMessageBusSerialiser _serialiser;
        private readonly ISystemProcessContext _systemProcessContext;
        private readonly ILogger<QueueSubscriber> _logger;

        private CancellationTokenSource _messageBusCts;
        private AwsResusableCancellationToken _token;

        public QueueSubscriber(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IMessageBusSerialiser serialiser,
            ISystemProcessContext systemProcessContext,
            ILogger<QueueSubscriber> logger)
        {
            _awsQueueClient = awsQueueClient;
            _awsConfiguration = awsConfiguration;
            _serialiser = serialiser ?? throw new ArgumentNullException(nameof(serialiser));
            _systemProcessContext = systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _logger.LogInformation($"QueueSubscriber initiating");
            _messageBusCts?.Cancel();
            _messageBusCts = new CancellationTokenSource();
            _token = new AwsResusableCancellationToken();

            _awsQueueClient.SubscribeToQueueAsync(
                _awsConfiguration.ScheduledRuleQueueName,
                async (s1, s2) => { await ExecuteCoordinationMessage(s1, s2); },
                _messageBusCts.Token,
                _token);

            _logger.LogInformation($"QueueSubscriber completed initiating");
        }

        public void Terminate()
        {
            _logger.LogInformation($"QueueSubscriber sent terminate signal to cancellation token reading message bus");
            _messageBusCts?.Cancel();
            _messageBusCts = null;
        }

        public async Task ExecuteCoordinationMessage(string messageId, string messageBody)
        {
            try
            {
                var opCtx = _systemProcessContext.CreateAndStartOperationContext();

                _logger.LogInformation($"QueueSubscriber read message {messageId} with body {messageBody} from {_awsConfiguration.ScheduledRuleQueueName} for operation {opCtx.Id}");

                var coordinateUpload = _serialiser.Deserialise<UploadCoordinatorMessage>(messageBody);

                if (coordinateUpload == null)
                {
                    _logger.LogError($"QueueSubscriber was unable to deserialise the message {messageId}");
                    opCtx.EndEventWithError($"QueueSubscriber was unable to deserialise the message {messageId}");
                    return;
                }



            }
            catch (Exception e)
            {
                _logger.LogError($"QueueSubscriber execute non distributed message encountered a top level exception.", e);
            }
        }
    }
}
