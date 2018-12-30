using System;
using System.Threading;
using System.Threading.Tasks;
using DomainV2.DTO.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.System.Auditing.Context.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Manager.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Services.Interfaces;
using Utilities.Aws_IO;
using Utilities.Aws_IO.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Services
{
    public class DataRequestsService : IDataRequestsService
    {
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly ISystemProcessContext _systemProcessContext;
        private readonly IThirdPartyDataRequestSerialiser _serialiser;
        private readonly IDataRequestManager _dataRequestManager;
        private readonly ILogger<DataRequestsService> _logger;

        private CancellationTokenSource _messageBusCts;
        private AwsResusableCancellationToken _token;

        public DataRequestsService(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            ISystemProcessContext systemProcessContext,
            IThirdPartyDataRequestSerialiser serialiser,
            IDataRequestManager dataRequestManager,
            ILogger<DataRequestsService> logger)
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
            _logger.LogInformation($"DataRequestsService initiate beginning");

            _messageBusCts?.Cancel();

            _messageBusCts = new CancellationTokenSource();
            _token = new AwsResusableCancellationToken();

            _awsQueueClient.SubscribeToQueueAsync(
                _awsConfiguration.DataSynchroniserRequestQueueName,
                async (s1, s2) => { await Execute(s1, s2); },
                _messageBusCts.Token,
                _token);

            _logger.LogInformation($"DataRequestsService initiate completed");
        }

        public void Terminate()
        {
            _logger.LogInformation($"DataRequestsService terminate beginning");

            _messageBusCts?.Cancel();
            _messageBusCts = null;

            _logger.LogInformation($"DataRequestsService terminate completed");
        }

        public async Task Execute(string messageId, string messageBody)
        {
            _logger.LogInformation($"DataRequestsService about to process a message with id of {messageId}");

            var opCtx = _systemProcessContext.CreateAndStartOperationContext();
            ISystemProcessOperationThirdPartyDataRequestContext dataCtx = null;

            try
            {
                var request = _serialiser.Deserialise(messageBody);
                dataCtx = opCtx.CreateAndStartDataRequestContext(messageId, request.SystemProcessOperationRuleRunId);

                if (!ValidateDataRequest(request.SystemProcessOperationRuleRunId))
                {
                    _logger.LogError($"DataRequestsService received a null or empty rule run id. Exiting");
                    return;
                }

                await _dataRequestManager.Handle(request.SystemProcessOperationRuleRunId, dataCtx);
            }
            catch (Exception e)
            {
                dataCtx?.EventError(e.Message);
            }
            finally
            {
                dataCtx?.EndEvent();
                opCtx.EndEvent();

                _logger.LogInformation($"DataRequestsService completed processing a message with id of {messageId}");
            }
        }

        private bool ValidateDataRequest(string id)
        {
            return !(string.IsNullOrWhiteSpace(id));
        }
    }
}
