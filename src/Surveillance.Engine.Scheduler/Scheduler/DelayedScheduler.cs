using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Surveillance.Aws;
using Domain.Surveillance.Scheduling;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Scheduler.Queues.Interfaces;
using Surveillance.Engine.Scheduler.Scheduler.Interfaces;

namespace Surveillance.Engine.Scheduler.Scheduler
{
    public class DelayedScheduler : IDelayedScheduler
    {
        private readonly IQueueScheduledRulePublisher _scheduledRulePublisher;
        private readonly IQueueDelayedRuleDistributedPublisher _distributedScheduledRulePublisher;
        private readonly ILogger<DelayedScheduler> _logger;

        public DelayedScheduler(
            IQueueScheduledRulePublisher scheduledRulePublisher,
            IQueueDelayedRuleDistributedPublisher distributedScheduledRulePublisher,
            ILogger<DelayedScheduler> logger)
        {
            _scheduledRulePublisher = scheduledRulePublisher ?? throw new ArgumentNullException(nameof(scheduledRulePublisher));
            _distributedScheduledRulePublisher = distributedScheduledRulePublisher ?? throw new ArgumentNullException(nameof(distributedScheduledRulePublisher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Save(AdHocScheduleRequest request)
        {
            if (request == null)
            {
                _logger.LogError($"save received a null ad hoc schedule request");
                return;
            }

            _logger.LogInformation($"save received a request from {request.OriginatingService}");



            _logger.LogInformation($"save completed a request from {request.OriginatingService}");
        }

        /// <summary>
        /// Check for due tasks and schedules them
        /// </summary>
        public void ScheduleDueTasks()
        {
            // fetch this from a database
            _logger.LogInformation($"schedule due tasks scanning repository for due tasks");
            var req = new List<AdHocScheduleRequest>();

            if (req == null || !req.Any())
            {
                _logger.LogInformation($"schedule due tasks scanning repository for due tasks found null or empty requests");
                return;
            }

            foreach (var request in req)
            {
                if (request == null)
                    continue;

                Schedule(request);
            }

            _logger.LogInformation($"schedule due tasks scanning repository for due tasks found null or empty requests");
        }

        private void Schedule(AdHocScheduleRequest request)
        {
            switch (request.Queue)
            {
                case SurveillanceSqsQueue.CaseMessage:
                    _logger.LogError($"schedule due tasks found a case message reschedule which is not supported");
                    break;
                case SurveillanceSqsQueue.DataImportS3Upload:
                    _logger.LogError($"schedule due tasks found a data import s3 upload reschedule which is not supported");
                    break;
                case SurveillanceSqsQueue.DataSynchroniserRequest:
                    _logger.LogError($"schedule due tasks found a data synchroniser request reschedule which is not supported");
                    break;
                case SurveillanceSqsQueue.DistributedRule:
                    RescheduleDistributedRule(request);
                    break;
                case SurveillanceSqsQueue.ScheduleRuleCancellation:
                    _logger.LogError($"schedule due tasks found schedule rule cancellation reschedule which is not supported");
                    break;
                case SurveillanceSqsQueue.ScheduledRule:
                    RescheduleScheduledRule(request);
                    break;
                case SurveillanceSqsQueue.TestRuleRunUpdate:
                    _logger.LogError($"schedule due tasks found a test rule run update reschedule which is not supported");
                    break;
                case SurveillanceSqsQueue.UploadCoordinator:
                    _logger.LogError($"schedule due tasks found a upload coordinator reschedule which is not supported");
                    break;
            }
        }

        private void RescheduleScheduledRule(AdHocScheduleRequest request)
        {
            _scheduledRulePublisher.Publish(request).Wait();
        }

        private void RescheduleDistributedRule(AdHocScheduleRequest request)
        {
            _distributedScheduledRulePublisher.Publish(request).Wait();
        }
    }
}
