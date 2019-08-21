namespace DataSynchroniser.Queues
{
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

    public class ScheduleRulePublisher : IScheduleRulePublisher
    {
        private readonly IAwsConfiguration _awsConfiguration;

        private readonly IAwsQueueClient _awsQueueClient;

        private readonly IRuleRunDataRequestRepository _dataRequestRepository;

        private readonly ILogger<ScheduleRulePublisher> _logger;

        private readonly IScheduledExecutionMessageBusSerialiser _messageBusSerialiser;

        private readonly ISystemProcessOperationRuleRunRepository _ruleRunRepository;

        public ScheduleRulePublisher(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IRuleRunDataRequestRepository dataRequestRepository,
            IScheduledExecutionMessageBusSerialiser messageBusSerialiser,
            ISystemProcessOperationRuleRunRepository ruleRunRepository,
            ILogger<ScheduleRulePublisher> logger)
        {
            this._awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            this._awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));

            this._dataRequestRepository =
                dataRequestRepository ?? throw new ArgumentNullException(nameof(dataRequestRepository));
            this._messageBusSerialiser =
                messageBusSerialiser ?? throw new ArgumentNullException(nameof(messageBusSerialiser));
            this._ruleRunRepository = ruleRunRepository ?? throw new ArgumentNullException(nameof(ruleRunRepository));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RescheduleRuleRun(
            string systemProcessOperationId,
            IReadOnlyCollection<MarketDataRequest> bmllRequests)
        {
            this._logger?.LogInformation($"{nameof(ScheduleRulePublisher)} beginning process");

            if (string.IsNullOrWhiteSpace(systemProcessOperationId))
            {
                this._logger.LogError($"{nameof(ScheduleRulePublisher)} had a null or empty rule run id. Returning");
                return;
            }

            var rulesToReschedule = await this._ruleRunRepository.Get(new[] { systemProcessOperationId });
            var filteredRescheduledRules = rulesToReschedule.GroupBy(i => i.RuleParameterId?.ToLower())
                .Select(x => x.FirstOrDefault()).ToList();

            foreach (var rule in filteredRescheduledRules) this.RescheduleRuleRun(rule);

            await this._dataRequestRepository.UpdateToCompleteWithDuplicates(bmllRequests);

            this._logger?.LogInformation($"{nameof(ScheduleRulePublisher)} completing process");
        }

        private void RescheduleRuleRun(ISystemProcessOperationRuleRun ruleRun)
        {
            if (ruleRun == null

                // ReSharper disable once MergeSequentialChecks
                || ruleRun.ScheduleRuleStart == null || ruleRun.ScheduleRuleEnd == null
                || string.IsNullOrWhiteSpace(ruleRun.RuleParameterId))
            {
                this._logger?.LogWarning(
                    $"{nameof(ScheduleRulePublisher)} received a badly formed rule run. Skipping.");
                return;
            }

            var scheduledExecution = new ScheduledExecution
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
            var serialisedMessage = this._messageBusSerialiser.SerialiseScheduledExecution(scheduledExecution);

            this._logger?.LogWarning(
                $"{nameof(ScheduleRulePublisher)} about to submit {serialisedMessage} to {this._awsConfiguration.ScheduleRuleDistributedWorkQueueName}");

            var sendToQueue = this._awsQueueClient.SendToQueue(
                this._awsConfiguration.ScheduleRuleDistributedWorkQueueName,
                serialisedMessage,
                cts.Token);

            sendToQueue.Wait(TimeSpan.FromMinutes(30));

            if (sendToQueue.IsCanceled)
                this._logger?.LogError(
                    $"{nameof(ScheduleRulePublisher)} timed out communicating with queue for {serialisedMessage} to {this._awsConfiguration.ScheduleRuleDistributedWorkQueueName}");

            this._logger?.LogWarning(
                $"{nameof(ScheduleRulePublisher)} completed submitting {serialisedMessage} to {this._awsConfiguration.ScheduleRuleDistributedWorkQueueName}");
        }
    }
}