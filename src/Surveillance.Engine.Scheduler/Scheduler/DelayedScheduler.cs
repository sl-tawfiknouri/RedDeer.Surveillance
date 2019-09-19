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

    /// <summary>
    /// The delayed scheduler.
    /// </summary>
    public class DelayedScheduler : IDelayedScheduler
    {
        /// <summary>
        /// The distributed scheduled rule publisher.
        /// </summary>
        private readonly IQueueDelayedRuleDistributedPublisher distributedScheduledRulePublisher;

        /// <summary>
        /// The scheduled rule publisher.
        /// </summary>
        private readonly IQueueScheduledRulePublisher scheduledRulePublisher;

        /// <summary>
        /// The task scheduler repository.
        /// </summary>
        private readonly ITaskSchedulerRepository taskSchedulerRepository;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<DelayedScheduler> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelayedScheduler"/> class.
        /// </summary>
        /// <param name="taskSchedulerRepository">
        /// The task scheduler repository.
        /// </param>
        /// <param name="scheduledRulePublisher">
        /// The scheduled rule publisher.
        /// </param>
        /// <param name="distributedScheduledRulePublisher">
        /// The distributed scheduled rule publisher.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public DelayedScheduler(
            ITaskSchedulerRepository taskSchedulerRepository,
            IQueueScheduledRulePublisher scheduledRulePublisher,
            IQueueDelayedRuleDistributedPublisher distributedScheduledRulePublisher,
            ILogger<DelayedScheduler> logger)
        {
            this.taskSchedulerRepository =
                taskSchedulerRepository ?? throw new ArgumentNullException(nameof(taskSchedulerRepository));
            this.scheduledRulePublisher =
                scheduledRulePublisher ?? throw new ArgumentNullException(nameof(scheduledRulePublisher));
            this.distributedScheduledRulePublisher =
                distributedScheduledRulePublisher ?? throw new ArgumentNullException(nameof(distributedScheduledRulePublisher));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The schedule due tasks.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task ScheduleDueTasksAsync()
        {
            this.logger.LogInformation("schedule due tasks scanning repository for due tasks");
            var tasks = await this.taskSchedulerRepository.ReadUnprocessedTask(DateTime.UtcNow).ConfigureAwait(false);

            if (tasks == null || !tasks.Any())
            {
                this.logger.LogInformation(
                    "schedule due tasks scanning repository for due tasks found null or empty requests");
                return;
            }

            foreach (var request in tasks)
            {
                if (request == null)
                {
                    continue;
                }

                await this.ScheduleAsync(request).ConfigureAwait(false);
            }

            await this.taskSchedulerRepository.MarkTasksProcessed(tasks).ConfigureAwait(false);

            this.logger.LogInformation("schedule due tasks scanning repository for due tasks found null or empty requests");
        }

        /// <summary>
        /// The schedule.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task ScheduleAsync(AdHocScheduleRequest request)
        {
            switch (request.Queue)
            {
                case SurveillanceSqsQueue.CaseMessage:
                    this.logger.LogError("schedule due tasks found a case message reschedule which is not supported");
                    break;
                case SurveillanceSqsQueue.DataImportS3Upload:
                    this.logger.LogError(
                        "schedule due tasks found a data import s3 upload reschedule which is not supported");
                    break;
                case SurveillanceSqsQueue.DataSynchroniserRequest:
                    this.logger.LogError(
                        "schedule due tasks found a data synchroniser request reschedule which is not supported");
                    break;
                case SurveillanceSqsQueue.DistributedRule:
                    await this.RescheduleDistributedRule(request).ConfigureAwait(false);
                    break;
                case SurveillanceSqsQueue.ScheduleRuleCancellation:
                    this.logger.LogError(
                        "schedule due tasks found schedule rule cancellation reschedule which is not supported");
                    break;
                case SurveillanceSqsQueue.ScheduledRule:
                    await this.RescheduleScheduledRule(request).ConfigureAwait(false);
                    break;
                case SurveillanceSqsQueue.TestRuleRunUpdate:
                    this.logger.LogError(
                        "schedule due tasks found a test rule run update reschedule which is not supported");
                    break;
                case SurveillanceSqsQueue.UploadCoordinator:
                    this.logger.LogError(
                        "schedule due tasks found a upload coordinator reschedule which is not supported");
                    break;
            }
        }

        /// <summary>
        /// The reschedule distributed rule.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task RescheduleDistributedRule(AdHocScheduleRequest request)
        {
            await this.distributedScheduledRulePublisher.Publish(request).ConfigureAwait(false);
        }

        /// <summary>
        /// The reschedule scheduled rule.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task RescheduleScheduledRule(AdHocScheduleRequest request)
        {
            await this.scheduledRulePublisher.Publish(request).ConfigureAwait(false);
        }
    }
}