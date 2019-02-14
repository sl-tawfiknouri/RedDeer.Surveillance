﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DomainV2.Scheduling;
using DomainV2.Scheduling.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.DataLayer.Processes.Interfaces;
using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Manager.Bmll.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.Bmll
{
    public class BmllDataRequestsRescheduleManager : IBmllDataRequestsRescheduleManager
    {
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;

        private readonly IRuleRunDataRequestRepository _dataRequestRepository;
        private readonly ISystemProcessOperationRuleRunRepository _ruleRunRepository;
        private readonly IScheduledExecutionMessageBusSerialiser _messageBusSerialiser;
        private readonly ILogger<BmllDataRequestsRescheduleManager> _logger;

        public BmllDataRequestsRescheduleManager(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IRuleRunDataRequestRepository dataRequestRepository,
            IScheduledExecutionMessageBusSerialiser messageBusSerialiser,
            ISystemProcessOperationRuleRunRepository ruleRunRepository,
            ILogger<BmllDataRequestsRescheduleManager> logger)
        {
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));

            _dataRequestRepository = dataRequestRepository ?? throw new ArgumentNullException(nameof(dataRequestRepository));
            _messageBusSerialiser = messageBusSerialiser ?? throw new ArgumentNullException(nameof(messageBusSerialiser));
            _ruleRunRepository = ruleRunRepository ?? throw new ArgumentNullException(nameof(ruleRunRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RescheduleRuleRun(string systemProcessOperationId, List<MarketDataRequestDataSource> bmllRequests)
        {
            _logger?.LogInformation($"BmllDataRequestsRescheduleManager beginning process");

            if (string.IsNullOrWhiteSpace(systemProcessOperationId))
            {
                _logger.LogError($"BmllDataRequestsRescheduleManager had a null or empty rule run id. Returning");
                return;
            }

            var rulesToReschedule = await _ruleRunRepository.Get(new[] {systemProcessOperationId});
            var filteredRescheduledRules = rulesToReschedule.GroupBy(i => i.RuleParameterId?.ToLower()).Select(x => x.FirstOrDefault()).ToList();
            
            foreach (var rule in filteredRescheduledRules)
            {
                RescheduleRuleRun(rule);
            }

            var req = bmllRequests?.Select(bm => bm.DataRequest).ToList();
            await _dataRequestRepository.UpdateToCompleteWithDuplicates(req);

            _logger?.LogInformation($"BmllDataRequestsRescheduleManager completing process");
        }

        private void RescheduleRuleRun(ISystemProcessOperationRuleRun ruleRun)
        {
            if (ruleRun == null
                // ReSharper disable once MergeSequentialChecks
                || ruleRun.ScheduleRuleStart == null
                || ruleRun.ScheduleRuleEnd == null
                || string.IsNullOrWhiteSpace(ruleRun.RuleParameterId))
            {
                _logger?.LogWarning($"BmllDataRequestsRescheduleManager received a badly formed rule run. Skipping.");
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

            _logger?.LogWarning($"BmllDataRequestsRescheduleManager about to submit {serialisedMessage} to {_awsConfiguration.ScheduleRuleDistributedWorkQueueName}");

            var sendToQueue = _awsQueueClient.SendToQueue(_awsConfiguration.ScheduleRuleDistributedWorkQueueName, serialisedMessage, cts.Token);

            sendToQueue.Wait(TimeSpan.FromMinutes(30));

            if (sendToQueue.IsCanceled)
            {
                _logger?.LogError($"BmllDataRequestsRescheduleManager timed out communicating with queue for {serialisedMessage} to {_awsConfiguration.ScheduleRuleDistributedWorkQueueName}");
            }

            _logger?.LogWarning($"BmllDataRequestsRescheduleManager completed submitting {serialisedMessage} to {_awsConfiguration.ScheduleRuleDistributedWorkQueueName}");
        }
    }
}
