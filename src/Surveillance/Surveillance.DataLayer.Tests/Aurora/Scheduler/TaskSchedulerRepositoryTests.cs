namespace Surveillance.DataLayer.Tests.Aurora.Scheduler
{
    using System;
    using System.Threading.Tasks;

    using Domain.Surveillance.Aws;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using Surveillance.DataLayer.Aurora;
    using Surveillance.DataLayer.Aurora.Scheduler;
    using Surveillance.DataLayer.Configuration.Interfaces;
    using Surveillance.DataLayer.Tests.Helpers;

    [TestFixture]
    public class TaskSchedulerRepositoryTests
    {
        private IDataLayerConfiguration _configuration;

        private ILogger<TaskSchedulerRepository> _logger;

        [Test]
        [Explicit("db integration")]
        public async Task Create_CreatesARow()
        {
            var connectionStringFactory = new ConnectionStringFactory(this._configuration);
            var repo = new TaskSchedulerRepository(connectionStringFactory, this._logger);
            var adhocRequest = new AdHocScheduleRequest
                                   {
                                       JsonSqsMessage = "abc",
                                       OriginatingService = "surv-main",
                                       Processed = false,
                                       Queue = SurveillanceSqsQueue.CaseMessage,
                                       ScheduleFor = DateTime.UtcNow
                                   };

            await repo.SaveTask(adhocRequest);
        }

        [Test]
        [Explicit("db integration")]
        public async Task Get_AdhocRequests()
        {
            var connectionStringFactory = new ConnectionStringFactory(this._configuration);
            var repo = new TaskSchedulerRepository(connectionStringFactory, this._logger);

            var result = await repo.ReadUnprocessedTask(DateTime.UtcNow);
        }

        [Test]
        [Explicit("db integration")]
        public async Task Get_And_Update_Task()
        {
            var connectionStringFactory = new ConnectionStringFactory(this._configuration);
            var repo = new TaskSchedulerRepository(connectionStringFactory, this._logger);

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

        [SetUp]
        public void Setup()
        {
            this._configuration = TestHelpers.Config();
            this._logger = new NullLogger<TaskSchedulerRepository>();
        }
    }
}