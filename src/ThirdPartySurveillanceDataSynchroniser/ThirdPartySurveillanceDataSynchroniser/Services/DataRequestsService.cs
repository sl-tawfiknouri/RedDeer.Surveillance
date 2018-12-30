using System;
using System.Threading;
using System.Threading.Tasks;
using DomainV2.DTO.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.System.Auditing.Context.Interfaces;
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
        private readonly ILogger<DataRequestsService> _logger;

        private CancellationTokenSource _messageBusCts;
        private AwsResusableCancellationToken _token;

        public DataRequestsService(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            ISystemProcessContext systemProcessContext,
            IThirdPartyDataRequestSerialiser serialiser,
            ILogger<DataRequestsService> logger)
        {
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            _systemProcessContext = systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));
            _serialiser = serialiser ?? throw new ArgumentNullException(nameof(serialiser));
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

                // so how do we deal with this? 
                // I think the better design is to have a manager which processes the request at this stage

                // i.e. has a repository, goes fetches the relevant rows and completes the logic




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
