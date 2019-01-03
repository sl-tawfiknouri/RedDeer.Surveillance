using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DomainV2.Scheduling;
using DomainV2.Scheduling.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.System.DataLayer.Processes.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Manager.Bmll.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.Bmll
{
    public class BmllDataRequestsRescheduleManager : IBmllDataRequestsRescheduleManager
    {
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;

        private readonly IScheduledExecutionMessageBusSerialiser _messageBusSerialiser;
        private readonly ISystemProcessOperationRuleRunRepository _ruleRunRepository;
        private readonly ILogger<BmllDataRequestsRescheduleManager> _logger;

        public BmllDataRequestsRescheduleManager(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IScheduledExecutionMessageBusSerialiser messageBusSerialiser,
            ISystemProcessOperationRuleRunRepository ruleRunRepository,
            ILogger<BmllDataRequestsRescheduleManager> logger)
        {
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            _messageBusSerialiser = messageBusSerialiser ?? throw new ArgumentNullException(nameof(messageBusSerialiser));
            _ruleRunRepository = ruleRunRepository ?? throw new ArgumentNullException(nameof(ruleRunRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RescheduleRuleRun(List<MarketDataRequestDataSource> bmllRequests)
        {
            _logger?.LogInformation($"BmllDataRequestsRescheduleManager beginning process");

            if (bmllRequests == null
                || !bmllRequests.Any())
            {
                _logger?.LogInformation($"BmllDataRequestsRescheduleManager completing process after null or no bmll requests passed to it");
                return;
            }

            var ruleRunIds = RescheduleRuleRunIds(bmllRequests);

            if (!ruleRunIds.Any())
            {
                _logger?.LogInformation($"BmllDataRequestsRescheduleManager completing process");
                return;
            }

            var rulesToReschedule = await _ruleRunRepository.Get(ruleRunIds);

            foreach (var rule in rulesToReschedule)
            {
                await RescheduleRuleRun(rule);
            }

            _logger?.LogInformation($"BmllDataRequestsRescheduleManager completing process");
        }

        private IReadOnlyCollection<string> RescheduleRuleRunIds(List<MarketDataRequestDataSource> marketData)
        {
            if (marketData == null
                || !marketData.Any())
            {
                return new string[0];
            }

            var ids =
                marketData
                    .GroupBy(i => i.DataRequest?.SystemProcessOperationRuleRunId)
                    .Select(x => x.Key)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList();

            return ids;
        }

        private async Task RescheduleRuleRun(ISystemProcessOperationRuleRun ruleRun)
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

            _logger?.LogWarning($"BmllDataRequestsRescheduleManager about to submit {serialisedMessage} to {_awsConfiguration.ScheduledRuleQueueName}");

            await _awsQueueClient.SendToQueue(_awsConfiguration.ScheduledRuleQueueName, serialisedMessage, cts.Token);

            _logger?.LogWarning($"BmllDataRequestsRescheduleManager completed submitting {serialisedMessage} to {_awsConfiguration.ScheduledRuleQueueName}");
        }
    }
}
