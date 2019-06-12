using System;
using NUnit.Framework;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.DataLayer.Tests.Helpers;
using System.Threading.Tasks;
using Domain.Surveillance.Aws;
using Domain.Surveillance.Scheduling;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Surveillance.DataLayer.Aurora;
using Surveillance.DataLayer.Aurora.Scheduler;

namespace Surveillance.DataLayer.Tests.Aurora.Scheduler
{
    [TestFixture]
    public class TaskSchedulerRepositoryTests
    {
        private IDataLayerConfiguration _configuration;
        private ILogger<TaskSchedulerRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _configuration = TestHelpers.Config();
            _logger = new NullLogger<TaskSchedulerRepository>();
        }

        [Test]
        [Explicit("db integration")]
        public async Task Create_CreatesARow()
        {
            var connectionStringFactory = new ConnectionStringFactory(_configuration);
            var repo = new TaskSchedulerRepository(connectionStringFactory, _logger);
            var adhocRequest = new AdHocScheduleRequest
            {
                JsonSqsMessage = "abc", OriginatingService = "surv-main", Processed = false,
                Queue = SurveillanceSqsQueue.CaseMessage, ScheduleFor = DateTime.UtcNow
            };

            await repo.SaveTask(adhocRequest);
        }

        [Test]
        [Explicit("db integration")]
        public async Task Get_AdhocRequests()
        {
            var connectionStringFactory = new ConnectionStringFactory(_configuration);
            var repo = new TaskSchedulerRepository(connectionStringFactory, _logger);

            var result = await repo.ReadUnprocessedTask(DateTime.UtcNow);
        }

        [Test]
        [Explicit("db integration")]
        public async Task Get_And_Update_Task()
        {
            var connectionStringFactory = new ConnectionStringFactory(_configuration);
            var repo = new TaskSchedulerRepository(connectionStringFactory, _logger);

            var adhocRequest = new AdHocScheduleRequest
            {
                JsonSqsMessage = "abc",
                OriginatingService = "surv-main",
                Processed = false,
                Queue = SurveillanceSqsQueue.CaseMessage,
                ScheduleFor = DateTime.UtcNow
            };

            await repo.SaveTask(adhocRequest);
            var result = await repo.ReadUnprocessedTask(DateTime.UtcNow);
            await repo.MarkTasksProcessed(result);
        }
    }
}
