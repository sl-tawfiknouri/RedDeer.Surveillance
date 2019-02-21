using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Domain.Scheduling;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Auditing.DataLayer.Interfaces;
using Surveillance.Auditing.DataLayer.Processes;
using Surveillance.Auditing.DataLayer.Repositories;
using Surveillance.Auditing.DataLayer.Repositories.Interfaces;

namespace Surveillance.Auditing.DataLayer.Tests.Repositories
{
    [TestFixture]
    public class SystemProcessOperationRuleRunRepositoryTests
    {
        private ISystemDataLayerConfig _config;
        private ILogger<ISystemProcessRepository> _processLogger;
        private ILogger<ISystemProcessOperationRepository> _operationLogger;
        private ILogger<ISystemProcessOperationRuleRunRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _config = A.Fake<ISystemDataLayerConfig>();
            _processLogger = A.Fake<ILogger<ISystemProcessRepository>>();
            _operationLogger = A.Fake<ILogger<ISystemProcessOperationRepository>>();
            _logger = A.Fake<ILogger<ISystemProcessOperationRuleRunRepository>>();

            A
                .CallTo(() => _config.SurveillanceAuroraConnectionString)
                .Returns("server=dev-temporary.cgedh3fdlw42.eu-west-1.rds.amazonaws.com; port=3306;uid=hackinguser;pwd='WillDelete3101';database=hackingdb1; Allow User Variables=True");
        }

        [Test]
        [Explicit]
        public async Task Create_RuleRun_InsertsIntoDb()
        {
            var connFactory = new ConnectionStringFactory(_config);
            var processRepository = new SystemProcessRepository(connFactory, _processLogger);
            var operationRepository = new SystemProcessOperationRepository(connFactory, _operationLogger);
            var ruleRunRepository = new SystemProcessOperationRuleRunRepository(connFactory, _logger);

            var systemProcess = new SystemProcess
            {
                MachineId = Environment.MachineName,
                ProcessId = Process.GetCurrentProcess().Id.ToString(),
                InstanceInitiated = DateTime.UtcNow,
                Heartbeat = DateTime.UtcNow,
                SystemProcessType = SystemProcessType.DataImportService
            };
            systemProcess.Id = systemProcess.GenerateInstanceId();

            await processRepository.Create(systemProcess);

            var systemProcessOperation = new SystemProcessOperation
            {
                SystemProcessId = systemProcess.Id,
                OperationStart = DateTime.UtcNow,
                OperationState = OperationState.InProcess
            };

            await operationRepository.Create(systemProcessOperation);

            var systemProcessOperationRuleRun = new SystemProcessOperationRuleRun
            {
                SystemProcessOperationId = systemProcessOperation.Id,
                RuleDescription = "High Profits",
                RuleVersion = "1.0",
                ScheduleRuleStart = DateTime.UtcNow,
                ScheduleRuleEnd = DateTime.UtcNow.AddMinutes(1)
            };

            await ruleRunRepository.Create(systemProcessOperationRuleRun);

            Assert.IsTrue(true);
        }

        [Test]
        [Explicit]
        public async Task Update_RuleRun_UpdatesInDb()
        {
            var connFactory = new ConnectionStringFactory(_config);
            var processRepository = new SystemProcessRepository(connFactory, _processLogger);
            var operationRepository = new SystemProcessOperationRepository(connFactory, _operationLogger);
            var ruleRunRepository = new SystemProcessOperationRuleRunRepository(connFactory, _logger);

            var systemProcess = new SystemProcess
            {
                MachineId = Environment.MachineName,
                ProcessId = Process.GetCurrentProcess().Id.ToString(),
                InstanceInitiated = DateTime.UtcNow,
                Heartbeat = DateTime.UtcNow,
                SystemProcessType = SystemProcessType.DataImportService
            };
            systemProcess.Id = systemProcess.GenerateInstanceId();

            await processRepository.Create(systemProcess);

            var systemProcessOperation = new SystemProcessOperation
            {
                SystemProcessId = systemProcess.Id,
                OperationStart = DateTime.UtcNow,
                OperationState = OperationState.InProcess
            };

            await operationRepository.Create(systemProcessOperation);

            var systemProcessOperationRuleRun = new SystemProcessOperationRuleRun
            {
                SystemProcessOperationId = systemProcessOperation.Id,
                RuleDescription = "High Profits",
                RuleVersion = "1.0",
                ScheduleRuleStart = DateTime.UtcNow,
                ScheduleRuleEnd = DateTime.UtcNow.AddMinutes(1)
            };

            await ruleRunRepository.Create(systemProcessOperationRuleRun);
            await ruleRunRepository.Update(systemProcessOperationRuleRun);

            Assert.IsTrue(true);
        }

        [Test]
        [Explicit]
        public async Task Get_RuleRun_UpdatesInDb()
        {
            var connFactory = new ConnectionStringFactory(_config);
            var processRepository = new SystemProcessRepository(connFactory, _processLogger);
            var operationRepository = new SystemProcessOperationRepository(connFactory, _operationLogger);
            var ruleRunRepository = new SystemProcessOperationRuleRunRepository(connFactory, _logger);

            var systemProcess = new SystemProcess
            {
                MachineId = Environment.MachineName,
                ProcessId = Process.GetCurrentProcess().Id.ToString(),
                InstanceInitiated = DateTime.UtcNow,
                Heartbeat = DateTime.UtcNow,
                SystemProcessType = SystemProcessType.DataImportService
            };
            systemProcess.Id = systemProcess.GenerateInstanceId();

            await processRepository.Create(systemProcess);

            var systemProcessOperation = new SystemProcessOperation
            {
                SystemProcessId = systemProcess.Id,
                OperationStart = DateTime.UtcNow,
                OperationState = OperationState.InProcess
            };

            await operationRepository.Create(systemProcessOperation);

            var systemProcessOperationRuleRun = new SystemProcessOperationRuleRun
            {
                SystemProcessOperationId = systemProcessOperation.Id,
                RuleDescription = "High Profits",
                RuleVersion = "1.0",
                ScheduleRuleStart = DateTime.UtcNow,
                ScheduleRuleEnd = DateTime.UtcNow.AddMinutes(1),
                RuleParameterId = "1",
                IsBackTest = true,
                RuleTypeId = (int)Rules.HighVolume
            };

            await ruleRunRepository.Create(systemProcessOperationRuleRun);
            await ruleRunRepository.Update(systemProcessOperationRuleRun);

            var etc = await ruleRunRepository.Get(new[] { systemProcessOperationRuleRun.Id.ToString()});

            Assert.IsTrue(true);
        }
    }
}
