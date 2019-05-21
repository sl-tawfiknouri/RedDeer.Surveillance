using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Runtime.SharedInterfaces;
using Infrastructure.Network.Aws.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Auditing.DataLayer.Processes.Interfaces;
using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
using Surveillance.Engine.Rules.Config.Interfaces;
using Surveillance.Engine.Rules.Queues.Interfaces;

namespace Surveillance.Engine.Rules.Queues
{
    public class QueueRuleCancellationInfrastructureBuilder : IQueueRuleCancellationInfrastructureBuilder
    {
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsSnsClient _awsSnsClient;
        private readonly ISystemProcessContext _systemProcess;
        private readonly IRuleEngineConfiguration _ruleEngineConfiguration;
        private readonly ISystemProcessRepository _systemProcessRepository;
        private readonly ILogger<IQueueRuleCancellationInfrastructureBuilder> _logger;

        public QueueRuleCancellationInfrastructureBuilder(
            IAwsQueueClient awsQueueClient,
            IAwsSnsClient awsSnsClient,
            ISystemProcessContext systemProcess,
            IRuleEngineConfiguration ruleEngineConfiguration,
            ISystemProcessRepository systemProcessRepository,
            Logger<IQueueRuleCancellationInfrastructureBuilder> logger)
        {
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsSnsClient = awsSnsClient ?? throw new ArgumentNullException(nameof(awsSnsClient));
            _systemProcess = systemProcess ?? throw new ArgumentNullException(nameof(systemProcess));
            _ruleEngineConfiguration = ruleEngineConfiguration ?? throw new ArgumentNullException(nameof(ruleEngineConfiguration));
            _systemProcessRepository = systemProcessRepository ?? throw new ArgumentNullException(nameof(systemProcessRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Setup()
        {
            _logger?.LogInformation($"Setup {nameof(QueueRuleCancellationInfrastructureBuilder)} initiating");

            await RemoveDeadProcessesQueues();
            var topicArn = await CreateSnsTopic();
            var sqsQueue = await CreateInstanceSqsQueue();
            await SubscribeSqsQueueToSns(topicArn, _awsQueueClient.SqsClient, sqsQueue);

            _logger?.LogInformation($"Setup {nameof(QueueRuleCancellationInfrastructureBuilder)} completed");
        }

        private async Task RemoveDeadProcessesQueues()
        {
            var expiredQueues = await _systemProcessRepository.ExpiredProcessesWithQueues();

            if (!expiredQueues.Any())
            {
                return;
            }

            foreach (var queue in expiredQueues)
            {
                var queueName = CancellationQueueNameBuilder(queue);
                _awsQueueClient.DeleteQueue(queueName).Wait();

                queue.CancelRuleQueueDeletedFlag = true;
                _systemProcessRepository.Update(queue).Wait();
            }
        }

        private async Task<string> CreateInstanceSqsQueue()
        {
            var queueName = CancellationQueueNameBuilder();
            var queueExists = await _awsQueueClient.ExistsQueue(queueName);

            if (!queueExists)
            {
                var queueUrl = await _awsQueueClient.CreateQueue(queueName);
                return queueUrl;
            }
            else
            {
                var queueUrl = await _awsQueueClient.UrlQueue(queueName);
                return queueUrl;
            }
        }

        private async Task<string> CreateSnsTopic()
        {
            var cts = new CancellationTokenSource(1000 * 60);
            var topicArn = await _awsSnsClient.CreateSnsTopic(CancellationTopicNameBuilder(), cts.Token);

            return topicArn;
        }

        private async Task SubscribeSqsQueueToSns(string topicArn, ICoreAmazonSQS sqsClient, string sqsUrl)
        {
            if (string.IsNullOrWhiteSpace(topicArn)
                || string.IsNullOrWhiteSpace(sqsUrl))
            {
                _logger?.LogError($"{nameof(topicArn)} or {nameof(sqsUrl)} was null in subscribe sqs queue to sns");
                return;
            }

            await _awsSnsClient.SubscribeQueueToSnsTopic(topicArn, sqsClient, sqsUrl);
        }

        private string CancellationQueueNameBuilder()
        {
            return CancellationQueueNameBuilder(_systemProcess?.SystemProcess());
        }

        private string CancellationQueueNameBuilder(ISystemProcess systemProcess)
        {
            return $"{_ruleEngineConfiguration.Environment}-surveillance-{_ruleEngineConfiguration.Client}-rule-cancellation-{systemProcess.MachineId}";
        }

        private string CancellationTopicNameBuilder()
        {
            return $"{_ruleEngineConfiguration.Environment}-surveillance-{_ruleEngineConfiguration.Client}-rule-cancellation";
        }
    }
}
