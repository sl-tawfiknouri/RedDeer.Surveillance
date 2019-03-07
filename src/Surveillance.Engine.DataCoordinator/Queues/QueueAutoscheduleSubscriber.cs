using System;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Network.Aws_IO;
using Infrastructure.Network.Aws_IO.Interfaces;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Interfaces;
using SharedKernel.Contracts.Queues;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.DataCoordinator.Coordinator.Interfaces;
using Surveillance.Engine.DataCoordinator.Queues.Interfaces;

namespace Surveillance.Engine.DataCoordinator.Queues
{
    /// <summary>
    /// Enable push based auto scheduling
    /// </summary>
    public class QueueAutoscheduleSubscriber : IQueueSubscriber
    {
        private readonly IDataVerifier _dataVerifier;
        private readonly IAutoSchedule _autoSchedule;
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly IMessageBusSerialiser _serialiser;
        private readonly ISystemProcessContext _systemProcessContext;
        private readonly ILogger<QueueAutoscheduleSubscriber> _logger;

        private CancellationTokenSource _messageBusCts;
        private AwsResusableCancellationToken _token;

        public QueueAutoscheduleSubscriber(
            IDataVerifier dataVerifier,
            IAutoSchedule autoSchedule,
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IMessageBusSerialiser serialiser,
            ISystemProcessContext systemProcessContext,
            ILogger<QueueAutoscheduleSubscriber> logger)
        {
            _dataVerifier = dataVerifier ?? throw new ArgumentNullException(nameof(dataVerifier));
            _autoSchedule = autoSchedule ?? throw new ArgumentNullException(nameof(autoSchedule));
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
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
                _awsConfiguration.UploadCoordinatorQueueName,
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

                _logger.LogInformation($"QueueSubscriber read message {messageId} with body {messageBody} from {_awsConfiguration.UploadCoordinatorQueueName} for operation {opCtx.Id}");

                var coordinateUpload = Deserialise(messageBody);

                if (coordinateUpload == null)
                {
                    _logger.LogError($"QueueSubscriber was unable to deserialise the message {messageId}");
                    opCtx.EndEventWithError($"QueueSubscriber was unable to deserialise the message {messageId}");
                    return;
                }

                await _dataVerifier.Scan();
                await _autoSchedule.Scan();

                _logger.LogInformation($"QueueSubscriber completed processing message {messageId} with body {messageBody} from {_awsConfiguration.UploadCoordinatorQueueName} for operation {opCtx.Id}");
            }
            catch (Exception e)
            {
                _logger.LogError($"QueueSubscriber execute non distributed message encountered a top level exception.", e);
            }
        }

        private AutoScheduleMessage Deserialise(string messageBody)
        {
            try
            {
                return _serialiser.Deserialise<AutoScheduleMessage>(messageBody);
            }
            catch
            {
                return null;
            }
        }
    }
}
