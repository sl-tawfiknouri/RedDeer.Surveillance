namespace Surveillance.Auditing.DataLayer.Tests.Repositories
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Auditing.DataLayer.Interfaces;
    using Surveillance.Auditing.DataLayer.Processes;
    using Surveillance.Auditing.DataLayer.Repositories;
    using Surveillance.Auditing.DataLayer.Repositories.Interfaces;

    [TestFixture]
    public class SystemProcessOperationRepositoryTests
    {
        private ISystemDataLayerConfig _config;

        private ILogger<ISystemProcessOperationRepository> _operationLogger;

        private ILogger<ISystemProcessRepository> _processLogger;

        [Test]
        [Explicit]
        public async Task Create_Operation_AddsToDb()
        {
            var connFactory = new ConnectionStringFactory(this._config);
            var parentRepository = new SystemProcessRepository(connFactory, this._processLogger);
            var repository = new SystemProcessOperationRepository(connFactory, this._operationLogger);

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

        [SetUp]
        public void Setup()
        {
            this._config = A.Fake<ISystemDataLayerConfig>();
            this._processLogger = A.Fake<ILogger<ISystemProcessRepository>>();
            this._operationLogger = A.Fake<ILogger<ISystemProcessOperationRepository>>();
        }

        [Test]
        [Explicit]
        public async Task Update_Operation_AddsToDb()
        {
            var connFactory = new ConnectionStringFactory(this._config);
            var parentRepository = new SystemProcessRepository(connFactory, this._processLogger);
            var repository = new SystemProcessOperationRepository(connFactory, this._operationLogger);

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