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
    public class SystemProcessOperationDistributeRuleRepositoryTests
    {
        private ISystemDataLayerConfig _config;

        private ILogger<ISystemProcessOperationDistributeRuleRepository> _logger;

        private ILogger<ISystemProcessOperationRepository> _operationLogger;

        private ILogger<SystemProcessRepository> _processLogger;

        [Test]
        [Explicit]
        public async Task Create_InsertsADistributedRuleEntity_AsExpected()
        {
            var connFactory = new ConnectionStringFactory(this._config);
            var processRepository = new SystemProcessRepository(connFactory, this._processLogger);
            var operationRepository = new SystemProcessOperationRepository(connFactory, this._operationLogger);
            var distributeRepository = new SystemProcessOperationDistributeRuleRepository(connFactory, this._logger);

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

            var systemProcessOperationRuleRun = new SystemProcessOperationDistributeRule
                                                    {
                                                        SystemProcessOperationId = systemProcessOperation.Id,
                                                        ScheduleRuleInitialStart = DateTime.UtcNow,
                                                        ScheduleRuleInitialEnd = DateTime.UtcNow.AddMinutes(30),
                                                        RulesDistributed = "High Profits, Marking The Close"
                                                    };

            await distributeRepository.Create(systemProcessOperationRuleRun);

            Assert.IsTrue(true);
        }

        [SetUp]
        public void Setup()
        {
            this._config = A.Fake<ISystemDataLayerConfig>();
            this._processLogger = A.Fake<ILogger<SystemProcessRepository>>();
            this._operationLogger = A.Fake<ILogger<ISystemProcessOperationRepository>>();
            this._logger = A.Fake<ILogger<ISystemProcessOperationDistributeRuleRepository>>();
        }

        [Test]
        [Explicit]
        public async Task Update_UpdatesADistributedRuleEntity_AsExpected()
        {
            var connFactory = new ConnectionStringFactory(this._config);
            var processRepository = new SystemProcessRepository(connFactory, this._processLogger);
            var operationRepository = new SystemProcessOperationRepository(connFactory, this._operationLogger);
            var distributeRepository = new SystemProcessOperationDistributeRuleRepository(connFactory, this._logger);

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

            var systemProcessOperationRuleRun = new SystemProcessOperationDistributeRule
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