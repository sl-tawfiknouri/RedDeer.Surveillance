using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ThirdPartySurveillanceDataSynchroniser.Services.Interfaces;
using Utilities.Aws_IO;
using Utilities.Aws_IO.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Services
{
    public class DataRequestsService : IDataRequestsService
    {
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly ILogger<DataRequestsService> _logger;

        private CancellationTokenSource _messageBusCts;
        private AwsResusableCancellationToken _token;

        public DataRequestsService(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            ILogger<DataRequestsService> logger)
        {
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
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
        }
    }
}
