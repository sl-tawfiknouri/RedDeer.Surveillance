using System;
using System.Diagnostics;
using NUnit.Framework;
using System.Threading.Tasks;
using FakeItEasy;
using Surveillance.Systems.DataLayer.Processes;
using Surveillance.Systems.DataLayer.Repositories;
using Microsoft.Extensions.Logging;
using Surveillance.Systems.DataLayer.Interfaces;
using Surveillance.Systems.DataLayer.Repositories.Interfaces;

namespace Surveillance.Systems.DataLayer.Tests.Repositories
{
    [TestFixture]
    public class SystemProcessOperationRepositoryTests
    {
        private ISystemDataLayerConfig _config;
        private ILogger<ISystemProcessRepository> _processLogger;
        private ILogger<ISystemProcessOperationRepository> _operationLogger;

        [SetUp]
        public void Setup()
        {
            _config = A.Fake<ISystemDataLayerConfig>();
            _processLogger = A.Fake<ILogger<ISystemProcessRepository>>();
            _operationLogger = A.Fake<ILogger<ISystemProcessOperationRepository>>();
        }

        [Test]
        [Explicit]
        public async Task Create_Operation_AddsToDb()
        {
            var connFactory = new ConnectionStringFactory(_config);
            var parentRepository = new SystemProcessRepository(connFactory, _processLogger);
            var repository = new SystemProcessOperationRepository(connFactory, _operationLogger);

            var systemProcess = new SystemProcess
            {
                MachineId = Environment.MachineName,
                ProcessId = Process.GetCurrentProcess().Id.ToString(),
                InstanceInitiated = DateTime.UtcNow,
                Heartbeat = DateTime.UtcNow,
                SystemProcessType = SystemProcessType.DataImportService
            };
            systemProcess.Id = systemProcess.GenerateInstanceId();

            await parentRepository.Create(systemProcess);

            var systemProcessOperation = new SystemProcessOperation
            {
                SystemProcessId = systemProcess.Id,
                OperationStart = DateTime.UtcNow,
                OperationState = OperationState.InProcess
            };

            await repository.Create(systemProcessOperation);

            Assert.IsTrue(true);
        }

        [Test]
        [Explicit]
        public async Task Update_Operation_AddsToDb()
        {
            var connFactory = new ConnectionStringFactory(_config);
            var parentRepository = new SystemProcessRepository(connFactory, _processLogger);
            var repository = new SystemProcessOperationRepository(connFactory, _operationLogger);

            var systemProcess = new SystemProcess
            {
                MachineId = Environment.MachineName,
                ProcessId = Process.GetCurrentProcess().Id.ToString(),
                InstanceInitiated = DateTime.UtcNow,
                Heartbeat = DateTime.UtcNow,
                SystemProcessType = SystemProcessType.DataImportService
            };
            systemProcess.Id = systemProcess.GenerateInstanceId();

            await parentRepository.Create(systemProcess);

            var systemProcessOperation = new SystemProcessOperation
            {
                SystemProcessId = systemProcess.Id,
                OperationStart = DateTime.UtcNow,
                OperationState = OperationState.InProcess
            };

            await repository.Create(systemProcessOperation);

            systemProcessOperation.OperationEnd = DateTime.UtcNow;

            await repository.Update(systemProcessOperation);

            Assert.IsTrue(true);
        }
    }
}
