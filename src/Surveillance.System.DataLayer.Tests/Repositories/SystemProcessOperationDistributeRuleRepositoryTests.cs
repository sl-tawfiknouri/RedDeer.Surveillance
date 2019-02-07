using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Systems.DataLayer.Interfaces;
using Surveillance.Systems.DataLayer.Processes;
using Surveillance.Systems.DataLayer.Repositories;
using Surveillance.Systems.DataLayer.Repositories.Interfaces;

namespace Surveillance.Systems.DataLayer.Tests.Repositories
{
    [TestFixture]
    public class SystemProcessOperationDistributeRuleRepositoryTests
    {
        private ISystemDataLayerConfig _config;
        private ILogger<ISystemProcessRepository> _processLogger;
        private ILogger<ISystemProcessOperationRepository> _operationLogger;
        private ILogger<ISystemProcessOperationDistributeRuleRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _config = A.Fake<ISystemDataLayerConfig>();
            _processLogger = A.Fake<ILogger<ISystemProcessRepository>>();
            _operationLogger = A.Fake<ILogger<ISystemProcessOperationRepository>>();
            _logger = A.Fake<ILogger<ISystemProcessOperationDistributeRuleRepository>>();
        }

        [Test]
        [Explicit]
        public async Task Create_InsertsADistributedRuleEntity_AsExpected()
        {
            var connFactory = new ConnectionStringFactory(_config);
            var processRepository = new SystemProcessRepository(connFactory, _processLogger);
            var operationRepository = new SystemProcessOperationRepository(connFactory, _operationLogger);
            var distributeRepository = new SystemProcessOperationDistributeRuleRepository(connFactory, _logger);

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

            var systemProcessOperationRuleRun = new SystemProcessOperationDistributeRule()
            {
                SystemProcessOperationId = systemProcessOperation.Id,
                ScheduleRuleInitialStart= DateTime.UtcNow,
                ScheduleRuleInitialEnd = DateTime.UtcNow.AddMinutes(30),
                RulesDistributed = "High Profits, Marking The Close"
            };

            await distributeRepository.Create(systemProcessOperationRuleRun);

            Assert.IsTrue(true);
        }

        [Test]
        [Explicit]
        public async Task Update_UpdatesADistributedRuleEntity_AsExpected()
        {
            var connFactory = new ConnectionStringFactory(_config);
            var processRepository = new SystemProcessRepository(connFactory, _processLogger);
            var operationRepository = new SystemProcessOperationRepository(connFactory, _operationLogger);
            var distributeRepository = new SystemProcessOperationDistributeRuleRepository(connFactory, _logger);

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

            var systemProcessOperationRuleRun = new SystemProcessOperationDistributeRule()
            {
                SystemProcessOperationId = systemProcessOperation.Id,
                ScheduleRuleInitialStart = DateTime.UtcNow,
                ScheduleRuleInitialEnd = DateTime.UtcNow.AddMinutes(30),
                RulesDistributed = "High Profits, Marking The Close"
            };

            await distributeRepository.Create(systemProcessOperationRuleRun);

            systemProcessOperationRuleRun.RulesDistributed = "Spoofing";

            await distributeRepository.Update(systemProcessOperationRuleRun);

            Assert.IsTrue(true);
        }
    }
}
