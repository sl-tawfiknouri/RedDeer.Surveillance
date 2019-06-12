using System;
using System.Collections.Generic;
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
        private readonly ILogger<DelayedScheduler> _logger;

        public DelayedScheduler(
            IQueueScheduledRulePublisher scheduledRulePublisher,
            ILogger<DelayedScheduler> logger)
        {
            _scheduledRulePublisher = scheduledRulePublisher ?? throw new ArgumentNullException(nameof(scheduledRulePublisher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Check for due tasks and schedules them
        /// </summary>
        public void ScheduleDueTasks()
        {
            // fetch this from a database
            var req = new List<AdHocScheduleRequest>();

            foreach (var request in req)
            {
                if (request == null)
                    continue;

                Schedule(request);
            }
        }

        private void Schedule(AdHocScheduleRequest request)
        {
            switch (request.Queue)
            {
                case SurveillanceSqsQueue.CaseMessage:
                    break;
                case SurveillanceSqsQueue.DataImportS3Upload:
                    break;
                case SurveillanceSqsQueue.DataSynchroniserRequest:
                    break;
                case SurveillanceSqsQueue.DistributedRule:
                    RescheduleDistributedRule(request);
                    break;
                case SurveillanceSqsQueue.ScheduleRuleCancellation:
                    break;
                case SurveillanceSqsQueue.ScheduledRule:
                    RescheduleScheduledRule(request);
                    break;
                case SurveillanceSqsQueue.TestRuleRunUpdate:
                    break;
                case SurveillanceSqsQueue.UploadCoordinator:
                    break;
            }
        }

        private void RescheduleScheduledRule(AdHocScheduleRequest request)
        {
            _scheduledRulePublisher.Publish(request).Wait();
        }

        private void RescheduleDistributedRule(AdHocScheduleRequest request)
        {

        }
    }
}
