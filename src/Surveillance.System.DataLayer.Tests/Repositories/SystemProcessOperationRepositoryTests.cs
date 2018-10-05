using System;
using System.Diagnostics;
using NUnit.Framework;
using System.Threading.Tasks;
using FakeItEasy;
using Surveillance.System.DataLayer.Processes;
using Surveillance.System.DataLayer.Repositories;
using Microsoft.Extensions.Logging;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.DataLayer.Tests.Repositories
{
    [TestFixture]
    public class SystemProcessOperationRepositoryTests
    {
        private ILogger<ISystemProcessRepository> _processLogger;
        private ILogger<ISystemProcessOperationRepository> _operationLogger;

        [SetUp]
        public void Setup()
        {
            _processLogger = A.Fake<ILogger<ISystemProcessRepository>>();
            _operationLogger = A.Fake<ILogger<ISystemProcessOperationRepository>>();
        }

        [Test]
        [Explicit]
        public async Task Create_Operation_AddsToDb()
        {
            var connFactory = new ConnectionStringFactory();
            var parentRepository = new SystemProcessRepository(connFactory, _processLogger);
            var repository = new SystemProcessOperationRepository(connFactory, _operationLogger);

            var systemProcess = new SystemProcess
            {
                MachineId = Environment.MachineName,
                ProcessId = Process.GetCurrentProcess().Id.ToString(),
                InstanceInitiated = DateTime.Now,
                Heartbeat = DateTime.Now,
                SystemProcessType = SystemProcessType.RelayService
            };
            systemProcess.Id = systemProcess.GenerateInstanceId();

            await parentRepository.Create(systemProcess);

            var systemProcessOperation = new SystemProcessOperation
            {
                SystemProcessId = systemProcess.Id,
                OperationStart = DateTime.Now,
                OperationState = OperationState.InProcess
            };

            await repository.Create(systemProcessOperation);

            Assert.IsTrue(true);
        }

        [Test]
        [Explicit]
        public async Task Update_Operation_AddsToDb()
        {
            var connFactory = new ConnectionStringFactory();
            var parentRepository = new SystemProcessRepository(connFactory, _processLogger);
            var repository = new SystemProcessOperationRepository(connFactory, _operationLogger);

            var systemProcess = new SystemProcess
            {
                MachineId = Environment.MachineName,
                ProcessId = Process.GetCurrentProcess().Id.ToString(),
                InstanceInitiated = DateTime.Now,
                Heartbeat = DateTime.Now,
                SystemProcessType = SystemProcessType.RelayService
            };
            systemProcess.Id = systemProcess.GenerateInstanceId();

            await parentRepository.Create(systemProcess);

            var systemProcessOperation = new SystemProcessOperation
            {
                SystemProcessId = systemProcess.Id,
                OperationStart = DateTime.Now,
                OperationState = OperationState.InProcess
            };

            await repository.Create(systemProcessOperation);

            systemProcessOperation.OperationEnd = DateTime.Now;

            await repository.Update(systemProcessOperation);

            Assert.IsTrue(true);
        }
    }
}
