namespace Surveillance.DataLayer.Aurora.Scheduler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Dapper;

    using Domain.Surveillance.Aws;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.Interfaces;
    using Surveillance.DataLayer.Aurora.Scheduler.Interfaces;

    public class TaskSchedulerRepository : ITaskSchedulerRepository
    {
        private const string MarkTaskAsProcessed = @"UPDATE AdHocScheduleRequest SET Processed = 1 WHERE Id = @Id;";

        private const string ReadTaskScheduler =
            @"SELECT Id, ScheduleFor, QueueId, JsonSqsMessage, OriginatingService, Processed FROM AdHocScheduleRequest WHERE Processed = 0 AND ScheduleFor <= @DueBy;";

        private const string SaveTaskScheduler =
            @"INSERT INTO AdHocScheduleRequest(ScheduleFor, QueueId, JsonSqsMessage, OriginatingService, Processed) VALUES(@ScheduleFor, @QueueId, @JsonSqsMessage, @OriginatingService, @Processed);";

        private readonly IConnectionStringFactory _dbConnectionFactory;

        private readonly ILogger<TaskSchedulerRepository> _logger;

        public TaskSchedulerRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<TaskSchedulerRepository> logger)
        {
            this._dbConnectionFactory =
                dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task MarkTasksProcessed(IReadOnlyCollection<AdHocScheduleRequest> requests)
        {
            requests = requests?.Where(_ => _ != null).ToList();

            if (requests == null || !requests.Any())
            {
                this._logger?.LogInformation("mark tasks as processed received a null or empty list of requests");
                return;
            }

            try
            {
                using (var dbConnection = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.ExecuteAsync(MarkTaskAsProcessed, requests))
                {
                    await conn;
                }
            }
            catch (Exception e)
            {
                this._logger?.LogError(
                    $"MarkTasksProcessed failed with exception {e.Message} {e?.InnerException?.Message}",
                    e);
            }
        }

        /// <summary>
        ///     Read tasks that are now matured
        /// </summary>
        public async Task<IReadOnlyCollection<AdHocScheduleRequest>> ReadUnprocessedTask(DateTime dueBy)
        {
            this._logger?.LogInformation($"due to read unprocessed tasks with cut off date of {dueBy}");

            var tasks = new List<AdHocScheduleRequest>();

            try
            {
                using (var dbConnection = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.QueryAsync<TaskSchedulerDto>(ReadTaskScheduler, new { DueBy = dueBy }))
                {
                    var result = await conn;
                    tasks = result.Select(_ => _.Map()).ToList();

                    this._logger?.LogInformation($"Fetched ReadUnprocessedTasks for {dueBy}");
                }
            }
            catch (Exception e)
            {
                this._logger?.LogError(
                    $"ReadUnprocessedTasks encountered an error for date {dueBy} {e.Message} {e?.InnerException?.Message}");
            }

            return tasks;
        }

        public async Task SaveTask(AdHocScheduleRequest request)
        {
            if (request == null)
            {
                this._logger?.LogInformation("request to save task is null");
                return;
            }

            var dto = new TaskSchedulerDto(request);

            try
            {
                this._logger?.LogInformation("saving ad hod schedule request");

                using (var dbConnection = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.ExecuteAsync(SaveTaskScheduler, dto))
                {
                    await conn;
                    this._logger.LogInformation("saving ad hoc schedule request completed");
                }
            }
            catch (Exception e)
            {
                this._logger.LogError($"SaveTask encountered error {e.Message} {e.InnerException?.Message}", e);
            }
        }

        /// <summary>
        ///     Leave setters in place for dapper
        /// </summary>
        private class TaskSchedulerDto
        {
            public TaskSchedulerDto()
            {
            }

            public TaskSchedulerDto(AdHocScheduleRequest request)
            {
                this.Id = request.Id;
                this.ScheduleFor = request.ScheduleFor;
                this.QueueId = (int)request.Queue;
                this.JsonSqsMessage = request.JsonSqsMessage;
                this.OriginatingService = request.OriginatingService;
            }

            public string Id { get; }

            public string JsonSqsMessage { get; }

            public string OriginatingService { get; }

            public bool Processed { get; set; }

            public int QueueId { get; }

            public DateTime ScheduleFor { get; }

            public AdHocScheduleRequest Map()
            {
                return new AdHocScheduleRequest
                           {
                               Id = this.Id,
                               JsonSqsMessage = this.JsonSqsMessage,
                               OriginatingService = this.OriginatingService,
                               Queue = (SurveillanceSqsQueue)this.QueueId,
                               ScheduleFor = this.ScheduleFor
                           };
            }
        }
    }
}