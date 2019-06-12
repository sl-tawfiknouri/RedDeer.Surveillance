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

namespace Surveillance.DataLayer.Aurora.Scheduler
{
    public class TaskSchedulerRepository : ITaskSchedulerRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<TaskSchedulerRepository> _logger;

        private const string SaveTaskScheduler =
            @"INSERT INTO AdHocScheduleRequest(ScheduleFor, QueueId, JsonSqsMessage, OriginatingService, Processed) VALUES(@ScheduleFor, @QueueId, @JsonSqsMessage, @OriginatingService, @Processed);";

        private const string ReadTaskScheduler = 
            @"SELECT Id, ScheduleFor, QueueId, JsonSqsMessage, OriginatingService, Processed FROM AdHocScheduleRequest WHERE Processed = 0 AND ScheduleFor < DueBy;";

        private const string MarkTaskAsProcessed =
            @"UPDATE AdHocScheduleRequest SET Processed = 1 WHERE Id = @Id;";

        public TaskSchedulerRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<TaskSchedulerRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SaveTask(AdHocScheduleRequest request)
        {
            if (request == null)
            {
                _logger?.LogInformation($"request to save task is null");
                return;
            }

            var dto = new TaskSchedulerDto(request);
            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();
                _logger?.LogInformation($"saving ad hod schedule request");

                using (var conn = dbConnection.ExecuteAsync(SaveTaskScheduler, dto))
                {
                    await conn;
                    _logger.LogInformation($"saving ad hoc schedule request completed");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"SaveTask encountered error {e.Message} {e.InnerException?.Message}", e);
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        /// <summary>
        /// Read tasks that are now matured
        /// </summary>
        public async Task<IReadOnlyCollection<AdHocScheduleRequest>> ReadUnprocessedTask(DateTime dueBy)
        {
            _logger?.LogInformation($"due to read unprocessed tasks with cut off date of {dueBy}");

            var dbConnection = _dbConnectionFactory.BuildConn();
            var tasks = new List<AdHocScheduleRequest>();

            try
            {
                dbConnection.Open();

                using (var conn = dbConnection.QueryAsync<TaskSchedulerDto>(ReadTaskScheduler, new { @DueBy = dueBy }))
                {
                    var result = await conn;
                    tasks = result.Select(_ => _.Map()).ToList();

                    _logger?.LogInformation($"Fetched ReadUnprocessedTasks for {dueBy}");
                }
            }
            catch (Exception e)
            {
                _logger?.LogError($"ReadUnprocessedTasks encountered an error for date {dueBy} {e.Message} {e?.InnerException?.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            return tasks;
        }

        public async Task MarkTasksProcessed(IReadOnlyCollection<AdHocScheduleRequest> requests)
        {
            requests = requests?.Where(_ => _ != null).ToList();

            if (requests == null || !requests.Any())
            {
                _logger?.LogInformation($"mark tasks as processed received a null or empty list of requests");
                return;
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                using (var conn = dbConnection.ExecuteAsync(MarkTaskAsProcessed, requests))
                {
                    await conn;
                }
            }
            catch (Exception e)
            {
                _logger?.LogError($"MarkTasksProcessed failed with exception {e.Message} {e?.InnerException?.Message}", e);
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        /// <summary>
        /// Leave setters in place for dapper
        /// </summary>
        private class TaskSchedulerDto
        {
            public TaskSchedulerDto()
            { }

            public TaskSchedulerDto(AdHocScheduleRequest request)
            {
                Id = request.Id;
                ScheduleFor = request.ScheduleFor;
                QueueId = (int)request.Queue;
                JsonSqsMessage = request.JsonSqsMessage;
                OriginatingService = request.OriginatingService;
            }

            public string Id { get; set; }
            public DateTime ScheduleFor { get; set; }
            public int QueueId { get; set; }
            public string JsonSqsMessage { get; set; }
            public string OriginatingService { get; set; }
            public bool Processed { get; set; }

            public AdHocScheduleRequest Map()
            {
                return new AdHocScheduleRequest
                {
                    Id = this.Id,
                    JsonSqsMessage = this.JsonSqsMessage,
                    OriginatingService = this.OriginatingService,
                    Queue = (SurveillanceSqsQueue) this.QueueId,
                    ScheduleFor = this.ScheduleFor
                };
            }
        }
    }
}
