namespace Surveillance.Engine.Rules.Analysis
{
    using System;
    using System.Threading.Tasks;

    using Domain.Surveillance.Aws;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using Surveillance.DataLayer.Aurora.Scheduler.Interfaces;
    using Surveillance.Engine.Rules.Analysis.Interfaces;

    public class TaskReSchedulerService : ITaskReSchedulerService
    {
        private readonly ILogger<TaskReSchedulerService> _logger;

        private readonly ITaskSchedulerRepository _taskSchedulerRepository;

        public TaskReSchedulerService(
            ITaskSchedulerRepository taskSchedulerRepository,
            ILogger<TaskReSchedulerService> logger)
        {
            this._taskSchedulerRepository = taskSchedulerRepository
                                            ?? throw new ArgumentNullException(nameof(taskSchedulerRepository));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Ensures that the rescheduled execution is also in the future as an additional layer
        ///     of business logic guards
        /// </summary>
        /// <param name="execution"></param>
        public async Task RescheduleFutureExecution(ScheduledExecution execution)
        {
            this._logger?.LogInformation("Beginning reschedule execution");

            if (execution == null)
            {
                this._logger?.LogInformation("Received null execution. Exiting");
                return;
            }

            if (execution.IsBackTest)
            {
                this._logger?.LogInformation("We don't reschedule back tests");
                return;
            }

            if (execution.IsForceRerun)
            {
                this._logger?.LogInformation("We don't reschedule re runs to prevent exponential growth in rule runs");
                return;
            }

            this._logger?.LogInformation("Serialising execution");
            var sqsMessage = JsonConvert.SerializeObject(execution);

            var projectedExecution = new AdHocScheduleRequest
                                         {
                                             OriginatingService = "Surveillance Engine",
                                             ScheduleFor = DateTime.UtcNow.AddDays(1), // re-run this tomorrow
                                             Queue =
                                                 SurveillanceSqsQueue
                                                     .DistributedRule, // avoids being re-distributed through the distributor
                                             JsonSqsMessage =
                                                 sqsMessage // message to rerun - must be what the json deserialiser is expecting!
                                         };

            this._logger?.LogInformation(
                $"About to save projected execution {nameof(AdHocScheduleRequest)} to the task scheduler repository");
            await this._taskSchedulerRepository.SaveTask(projectedExecution);
        }
    }
}