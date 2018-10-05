﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.System.DataLayer.Processes;
using Surveillance.System.DataLayer.Repositories;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.DataLayer.Tests.Repositories
{
    [TestFixture]
    public class SystemProcessOperationDistributeRuleRepositoryTests
    {
        private ILogger<ISystemProcessRepository> _processLogger;
        private ILogger<ISystemProcessOperationRepository> _operationLogger;
        private ILogger<ISystemProcessOperationDistributeRuleRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _processLogger = A.Fake<ILogger<ISystemProcessRepository>>();
            _operationLogger = A.Fake<ILogger<ISystemProcessOperationRepository>>();
            _logger = A.Fake<ILogger<ISystemProcessOperationDistributeRuleRepository>>();
        }

        [Test]
        [Explicit]
        public async Task Create_InsertsADistributedRuleEntity_AsExpected()
        {
            var connFactory = new ConnectionStringFactory();
            var processRepository = new SystemProcessRepository(connFactory, _processLogger);
            var operationRepository = new SystemProcessOperationRepository(connFactory, _operationLogger);
            var distributeRepository = new SystemProcessOperationDistributeRuleRepository(connFactory, _logger);

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
            var connFactory = new ConnectionStringFactory();
            var processRepository = new SystemProcessRepository(connFactory, _processLogger);
            var operationRepository = new SystemProcessOperationRepository(connFactory, _operationLogger);
            var distributeRepository = new SystemProcessOperationDistributeRuleRepository(connFactory, _logger);

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
