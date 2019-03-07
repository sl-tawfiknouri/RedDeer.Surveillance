using System;
using System.Threading;
using System.Threading.Tasks;
using DataSynchroniser.Manager.Interfaces;
using DataSynchroniser.Queues.Interfaces;
using Infrastructure.Network.Aws_IO;
using Infrastructure.Network.Aws_IO.Interfaces;
using Microsoft.Extensions.Logging;
using SharedKernel.Contracts.Queues.Interfaces;
using Surveillance.Auditing.Context.Interfaces;

namespace DataSynchroniser.Queues
{
    public class DataRequestSubscriber : IDataRequestSubscriber
    {
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly ISystemProcessContext _systemProcessContext;
        private readonly IThirdPartyDataRequestSerialiser _serialiser;
        private readonly IDataRequestManager _dataRequestManager;
        private readonly ILogger<DataRequestSubscriber> _logger;

        private CancellationTokenSource _messageBusCts;
        private AwsResusableCancellationToken _token;

        public DataRequestSubscriber(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            ISystemProcessContext systemProcessContext,
            IThirdPartyDataRequestSerialiser serialiser,
            IDataRequestManager dataRequestManager,
            ILogger<DataRequestSubscriber> logger)
        {
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            _systemProcessContext = systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));
            _serialiser = serialiser ?? throw new ArgumentNullException(nameof(serialiser));
            _dataRequestManager = dataRequestManager ?? throw new ArgumentNullException(nameof(dataRequestManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _logger.LogInformation($"{nameof(DataRequestSubscriber)} initiate beginning");

            _messageBusCts?.Cancel();

            _messageBusCts = new CancellationTokenSource();
            _token = new AwsResusableCancellationToken();

            _awsQueueClient.SubscribeToQueueAsync(
                _awsConfiguration.DataSynchroniserRequestQueueName,
                async (s1, s2) => { await Execute(s1, s2); },
                _messageBusCts.Token,
                _token);

            _logger.LogInformation($"{nameof(DataRequestSubscriber)} initiate completed");
        }

        public void Terminate()
        {
            _logger.LogInformation($"{nameof(DataRequestSubscriber)} terminate beginning");

            _messageBusCts?.Cancel();
            _messageBusCts = null;

            _logger.LogInformation($"{nameof(DataRequestSubscriber)} terminate completed");
        }

        public async Task Execute(string messageId, string messageBody)
        {
            _logger.LogInformation($"{nameof(DataRequestSubscriber)} about to process a message with id of {messageId}");

            var opCtx = _systemProcessContext.CreateAndStartOperationContext();
            ISystemProcessOperationThirdPartyDataRequestContext dataCtx = null;

            try
            {
                var request = _serialiser.Deserialise(messageBody);
                dataCtx = opCtx.CreateAndStartDataRequestContext(messageId, request.SystemProcessOperationId);

                if (!ValidateDataRequest(request.SystemProcessOperationId))
                {
                    _logger.LogError($"{nameof(DataRequestSubscriber)} received a null or empty system process operation id. Exiting");
                    return;
                }

                await _dataRequestManager.Handle(request.SystemProcessOperationId, dataCtx);
            }
            catch (Exception e)
            {
                dataCtx?.EventError(e.Message);
            }
            finally
            {
                dataCtx?.EndEvent();
                opCtx.EndEvent();

                _logger.LogInformation($"{nameof(DataRequestSubscriber)} completed processing a message with id of {messageId}");
            }
        }

        private bool ValidateDataRequest(string id)
        {
            return !(string.IsNullOrWhiteSpace(id));
        }
    }
}
