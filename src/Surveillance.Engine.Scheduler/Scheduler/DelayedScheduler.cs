namespace Surveillance.Engine.Scheduler.Scheduler
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Domain.Surveillance.Aws;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.Scheduler.Interfaces;
    using Surveillance.Engine.Scheduler.Queues.Interfaces;
    using Surveillance.Engine.Scheduler.Scheduler.Interfaces;

    public class DelayedScheduler : IDelayedScheduler
    {
        private readonly IQueueDelayedRuleDistributedPublisher _distributedScheduledRulePublisher;

        private readonly ILogger<DelayedScheduler> _logger;

        private readonly IQueueScheduledRulePublisher _scheduledRulePublisher;

        private readonly ITaskSchedulerRepository _taskSchedulerRepository;

        public DelayedScheduler(
            ITaskSchedulerRepository taskSchedulerRepository,
            IQueueScheduledRulePublisher scheduledRulePublisher,
            IQueueDelayedRuleDistributedPublisher distributedScheduledRulePublisher,
            ILogger<DelayedScheduler> logger)
        {
            this._taskSchedulerRepository = taskSchedulerRepository
                                            ?? throw new ArgumentNullException(nameof(taskSchedulerRepository));
            this._scheduledRulePublisher =
                scheduledRulePublisher ?? throw new ArgumentNullException(nameof(scheduledRulePublisher));
            this._distributedScheduledRulePublisher = distributedScheduledRulePublisher
                                                      ?? throw new ArgumentNullException(
                                                          nameof(distributedScheduledRulePublisher));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Check for due tasks and schedules them
        /// </summary>
        public async Task ScheduleDueTasks()
        {
            this._logger.LogInformation("schedule due tasks scanning repository for due tasks");
            var tasks = await this._taskSchedulerRepository.ReadUnprocessedTask(DateTime.UtcNow);

            if (tasks == null || !tasks.Any())
            {
                this._logger.LogInformation(
                    "schedule due tasks scanning repository for due tasks found null or empty requests");
                return;
            }

            foreach (var request in tasks)
            {
                if (request == null)
                    continue;

                this.Schedule(request);
            }

            await this._taskSchedulerRepository.MarkTasksProcessed(tasks);

            this._logger.LogInformation(
                "schedule due tasks scanning repository for due tasks found null or empty requests");
        }

        private void RescheduleDistributedRule(AdHocScheduleRequest request)
        {
            this._distributedScheduledRulePublisher.Publish(request).Wait();
        }

        private void RescheduleScheduledRule(AdHocScheduleRequest request)
        {
            this._scheduledRulePublisher.Publish(request).Wait();
        }

        private void Schedule(AdHocScheduleRequest request)
        {
            switch (request.Queue)
            {
                case SurveillanceSqsQueue.CaseMessage:
                    this._logger.LogError("schedule due tasks found a case message reschedule which is not supported");
                    break;
                case SurveillanceSqsQueue.DataImportS3Upload:
                    this._logger.LogError(
                        "schedule due tasks found a data import s3 upload reschedule which is not supported");
                    break;
                case SurveillanceSqsQueue.DataSynchroniserRequest:
                    this._logger.LogError(
                        "schedule due tasks found a data synchroniser request reschedule which is not supported");
                    break;
                case SurveillanceSqsQueue.DistributedRule:
                    this.RescheduleDistributedRule(request);
                    break;
                case SurveillanceSqsQueue.ScheduleRuleCancellation:
                    this._logger.LogError(
                        "schedule due tasks found schedule rule cancellation reschedule which is not supported");
                    break;
                case SurveillanceSqsQueue.ScheduledRule:
                    this.RescheduleScheduledRule(request);
                    break;
                case SurveillanceSqsQueue.TestRuleRunUpdate:
                    this._logger.LogError(
                        "schedule due tasks found a test rule run update reschedule which is not supported");
                    break;
                case SurveillanceSqsQueue.UploadCoordinator:
                    this._logger.LogError(
                        "schedule due tasks found a upload coordinator reschedule which is not supported");
                    break;
            }
        }
    }
}