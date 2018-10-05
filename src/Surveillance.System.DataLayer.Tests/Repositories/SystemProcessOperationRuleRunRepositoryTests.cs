using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.System.DataLayer.Interfaces;
using Surveillance.System.DataLayer.Processes;
using Surveillance.System.DataLayer.Repositories;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.DataLayer.Tests.Repositories
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
                InstanceInitiated = DateTime.Now,
                Heartbeat = DateTime.Now,
                SystemProcessType = SystemProcessType.RelayService
            };
            systemProcess.Id = systemProcess.GenerateInstanceId();

            await processRepository.Create(systemProcess);

            var systemProcessOperation = new SystemProcessOperation
            {
                SystemProcessId = systemProcess.Id,
                OperationStart = DateTime.Now,
                OperationState = OperationState.InProcess
            };

            await operationRepository.Create(systemProcessOperation);

            var systemProcessOperationRuleRun = new SystemProcessOperationRuleRun
            {
                SystemProcessOperationId = systemProcessOperation.Id,
                RuleDescription = "High Profits",
                RuleVersion = "1.0",
                Alerts = 3,
                ScheduleRuleStart = DateTime.Now,
                ScheduleRuleEnd = DateTime.Now.AddMinutes(1)
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
                InstanceInitiated = DateTime.Now,
                Heartbeat = DateTime.Now,
                SystemProcessType = SystemProcessType.RelayService
            };
            systemProcess.Id = systemProcess.GenerateInstanceId();

            await processRepository.Create(systemProcess);

            var systemProcessOperation = new SystemProcessOperation
            {
                SystemProcessId = systemProcess.Id,
                OperationStart = DateTime.Now,
                OperationState = OperationState.InProcess
            };

            await operationRepository.Create(systemProcessOperation);

            var systemProcessOperationRuleRun = new SystemProcessOperationRuleRun
            {
                SystemProcessOperationId = systemProcessOperation.Id,
                RuleDescription = "High Profits",
                RuleVersion = "1.0",
                Alerts = 3,
                ScheduleRuleStart = DateTime.Now,
                ScheduleRuleEnd = DateTime.Now.AddMinutes(1)
            };

            await ruleRunRepository.Create(systemProcessOperationRuleRun);

            systemProcessOperationRuleRun.Alerts = 5;

            await ruleRunRepository.Update(systemProcessOperationRuleRun);

            Assert.IsTrue(true);
        }
    }
}
