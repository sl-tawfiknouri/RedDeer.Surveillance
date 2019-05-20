using System;
using System.Threading.Tasks;
using Infrastructure.Network.Aws.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.DataLayer.Processes.Interfaces;
using Surveillance.Engine.Rules.Config.Interfaces;
using Surveillance.Engine.Rules.Queues.Interfaces;

namespace Surveillance.Engine.Rules.Queues
{
    public class QueueRuleCancellationInfrastructureBuilder : IQueueRuleCancellationInfrastructureBuilder
    {
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsSnsClient _awsSnsClient;
        private readonly ISystemProcess _systemProcess;
        private readonly IRuleEngineConfiguration _ruleEngineConfiguration;
        private readonly ILogger<IQueueRuleCancellationInfrastructureBuilder> _logger;

        public QueueRuleCancellationInfrastructureBuilder(
            IAwsQueueClient awsQueueClient,
            IAwsSnsClient awsSnsClient,
            ISystemProcess systemProcess,
            IRuleEngineConfiguration ruleEngineConfiguration,
            Logger<IQueueRuleCancellationInfrastructureBuilder> logger)
        {
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsSnsClient = awsSnsClient ?? throw new ArgumentNullException(nameof(awsSnsClient));
            _systemProcess = systemProcess ?? throw new ArgumentNullException(nameof(systemProcess));
            _ruleEngineConfiguration = ruleEngineConfiguration ?? throw new ArgumentNullException(nameof(ruleEngineConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Setup()
        {
            _logger?.LogInformation($"Setup {nameof(QueueRuleCancellationInfrastructureBuilder)} initiating");

            await RemoveDeadProcessesQueues();
            await CreateSnsTopic();
            await CreateInstanceSqsQueue();
            await SubscribeSqsQueueToSns();

            _logger?.LogInformation($"Setup {nameof(QueueRuleCancellationInfrastructureBuilder)} completed");
        }

        private async Task RemoveDeadProcessesQueues()
        {
        }

        private async Task CreateInstanceSqsQueue()
        {
            var queueExists = await _awsQueueClient.ExistsQueue(CancellationQueueNameBuilder());

            if (!queueExists)
            {
                await _awsQueueClient.CreateQueue(CancellationQueueNameBuilder());
            }
        }

        private async Task CreateSnsTopic()
        {
            // does sns topic exist?
            await _awsSnsClient.CreateSnsTopic();
        }

        private async Task SubscribeSqsQueueToSns()
        {
            await _awsSnsClient.SubscribeQueueToSnsTopic(new object());
        }

        private string CancellationQueueNameBuilder()
        {
            return $"{_ruleEngineConfiguration.Environment}-surveillance-{_ruleEngineConfiguration.Client}-rule-cancellation-{_systemProcess.MachineId}";
        }
    }
}
