using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataSynchroniser.Queues.Interfaces;
using Domain.Surveillance.Scheduling;
using Domain.Surveillance.Scheduling.Interfaces;
using Infrastructure.Network.Aws.Interfaces;
using Microsoft.Extensions.Logging;
using SharedKernel.Contracts.Markets;
using Surveillance.Auditing.DataLayer.Processes.Interfaces;
using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;

namespace DataSynchroniser.Queues
{
    public class ScheduleRulePublisher : IScheduleRulePublisher
    {
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;

        private readonly IRuleRunDataRequestRepository _dataRequestRepository;
        private readonly ISystemProcessOperationRuleRunRepository _ruleRunRepository;
        private readonly IScheduledExecutionMessageBusSerialiser _messageBusSerialiser;
        private readonly ILogger<ScheduleRulePublisher> _logger;

        public ScheduleRulePublisher(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IRuleRunDataRequestRepository dataRequestRepository,
            IScheduledExecutionMessageBusSerialiser messageBusSerialiser,
            ISystemProcessOperationRuleRunRepository ruleRunRepository,
            ILogger<ScheduleRulePublisher> logger)
        {
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));

            _dataRequestRepository = dataRequestRepository ?? throw new ArgumentNullException(nameof(dataRequestRepository));
            _messageBusSerialiser = messageBusSerialiser ?? throw new ArgumentNullException(nameof(messageBusSerialiser));
            _ruleRunRepository = ruleRunRepository ?? throw new ArgumentNullException(nameof(ruleRunRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RescheduleRuleRun(string systemProcessOperationId, IReadOnlyCollection<MarketDataRequest> bmllRequests)
        {
            _logger?.LogInformation($"{nameof(ScheduleRulePublisher)} beginning process");

            if (string.IsNullOrWhiteSpace(systemProcessOperationId))
            {
                _logger.LogError($"{nameof(ScheduleRulePublisher)} had a null or empty rule run id. Returning");
                return;
            }

            var rulesToReschedule = await _ruleRunRepository.Get(new[] {systemProcessOperationId});
            var filteredRescheduledRules = rulesToReschedule.GroupBy(i => i.RuleParameterId?.ToLower()).Select(x => x.FirstOrDefault()).ToList();
            
            foreach (var rule in filteredRescheduledRules)
            {
                RescheduleRuleRun(rule);
            }

            await _dataRequestRepository.UpdateToCompleteWithDuplicates(bmllRequests);

            _logger?.LogInformation($"{nameof(ScheduleRulePublisher)} completing process");
        }

        private void RescheduleRuleRun(ISystemProcessOperationRuleRun ruleRun)
        {
            if (ruleRun == null
                // ReSharper disable once MergeSequentialChecks
                || ruleRun.ScheduleRuleStart == null
                || ruleRun.ScheduleRuleEnd == null
                || string.IsNullOrWhiteSpace(ruleRun.RuleParameterId))
            {
                _logger?.LogWarning($"{nameof(ScheduleRulePublisher)} received a badly formed rule run. Skipping.");
                return;
            }

            var scheduledExecution =
                new ScheduledExecution
                {
                    Rules = new List<RuleIdentifier>
                    {
                        new RuleIdentifier
                        {
                            Ids = new[] { ruleRun.RuleParameterId },
                            Rule = (Rules)ruleRun.RuleTypeId
                        }
                    },
                    TimeSeriesInitiation = ruleRun.ScheduleRuleStart.GetValueOrDefault(),
                    TimeSeriesTermination = ruleRun.ScheduleRuleEnd.Value,
                    CorrelationId = ruleRun.CorrelationId,
                    IsBackTest = ruleRun.IsBackTest,
                    IsForceRerun = true
                };

            var cts = new CancellationTokenSource();
            var serialisedMessage = _messageBusSerialiser.SerialiseScheduledExecution(scheduledExecution);

            _logger?.LogWarning($"{nameof(ScheduleRulePublisher)} about to submit {serialisedMessage} to {_awsConfiguration.ScheduleRuleDistributedWorkQueueName}");

            var sendToQueue = _awsQueueClient.SendToQueue(_awsConfiguration.ScheduleRuleDistributedWorkQueueName, serialisedMessage, cts.Token);

            sendToQueue.Wait(TimeSpan.FromMinutes(30));

            if (sendToQueue.IsCanceled)
            {
                _logger?.LogError($"{nameof(ScheduleRulePublisher)} timed out communicating with queue for {serialisedMessage} to {_awsConfiguration.ScheduleRuleDistributedWorkQueueName}");
            }

            _logger?.LogWarning($"{nameof(ScheduleRulePublisher)} completed submitting {serialisedMessage} to {_awsConfiguration.ScheduleRuleDistributedWorkQueueName}");
        }
    }
}
