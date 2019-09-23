namespace DataSynchroniser.Queues
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using DataSynchroniser.Manager.Interfaces;
    using DataSynchroniser.Queues.Interfaces;

    using Infrastructure.Network.Aws;
    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Queues.Interfaces;

    using Surveillance.Auditing.Context.Interfaces;

    public class DataRequestSubscriber : IDataRequestSubscriber
    {
        private readonly IAwsConfiguration _awsConfiguration;

        private readonly IAwsQueueClient _awsQueueClient;

        private readonly IDataRequestManager _dataRequestManager;

        private readonly ILogger<DataRequestSubscriber> _logger;

        private readonly IThirdPartyDataRequestSerialiser _serialiser;

        private readonly ISystemProcessContext _systemProcessContext;

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
            this._awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            this._awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            this._systemProcessContext =
                systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));
            this._serialiser = serialiser ?? throw new ArgumentNullException(nameof(serialiser));
            this._dataRequestManager =
                dataRequestManager ?? throw new ArgumentNullException(nameof(dataRequestManager));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Execute(string messageId, string messageBody)
        {
            this._logger.LogInformation(
                $"{nameof(DataRequestSubscriber)} about to process a message with id of {messageId}");

            var opCtx = this._systemProcessContext.CreateAndStartOperationContext();
            ISystemProcessOperationThirdPartyDataRequestContext dataCtx = null;

            try
            {
                var request = this._serialiser.Deserialise(messageBody);
                dataCtx = opCtx.CreateAndStartDataRequestContext(messageId, request.SystemProcessOperationId);

                if (!this.ValidateDataRequest(request.SystemProcessOperationId))
                {
                    this._logger.LogError(
                        $"{nameof(DataRequestSubscriber)} received a null or empty system process operation id. Exiting");
                    return;
                }

                await this._dataRequestManager.Handle(request.SystemProcessOperationId, dataCtx);
            }
            catch (Exception e)
            {
                dataCtx?.EventError(e.Message);
            }
            finally
            {
                dataCtx?.EndEvent();
                opCtx.EndEvent();

                this._logger.LogInformation(
                    $"{nameof(DataRequestSubscriber)} completed processing a message with id of {messageId}");
            }
        }

        public void Initiate()
        {
            this._logger.LogInformation($"{nameof(DataRequestSubscriber)} initiate beginning");

            this._messageBusCts?.Cancel();

            this._messageBusCts = new CancellationTokenSource();
            this._token = new AwsResusableCancellationToken();

            this._awsQueueClient.SubscribeToQueueAsync(
                this._awsConfiguration.DataSynchroniserRequestQueueName,
                async (s1, s2) => { await this.Execute(s1, s2); },
                this._messageBusCts.Token,
                this._token);

            this._logger.LogInformation($"{nameof(DataRequestSubscriber)} initiate completed");
        }

        public void Terminate()
        {
            this._logger.LogInformation($"{nameof(DataRequestSubscriber)} terminate beginning");

            this._messageBusCts?.Cancel();
            this._messageBusCts = null;

            this._logger.LogInformation($"{nameof(DataRequestSubscriber)} terminate completed");
        }

        private bool ValidateDataRequest(string id)
        {
            return !string.IsNullOrWhiteSpace(id);
        }
    }
}