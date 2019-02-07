﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Systems.DataLayer.Interfaces;
using Surveillance.Systems.DataLayer.Processes;
using Surveillance.Systems.DataLayer.Repositories;

namespace Surveillance.Systems.DataLayer.Tests.Repositories
{
    [TestFixture]
    public class SystemProcessRepositoryTests
    {
        private ISystemDataLayerConfig _config;
        private ILogger<SystemProcessRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _config = A.Fake<ISystemDataLayerConfig>();
            _logger = A.Fake<ILogger<SystemProcessRepository>>();
        }

        [Test]
        [Explicit]
        public async Task Create_InsertsRow_IntoDb()
        {
            var repo = new SystemProcessRepository(new ConnectionStringFactory(_config), _logger);
            var systemProcess = new SystemProcess
            {
                MachineId = Environment.MachineName,
                ProcessId = Process.GetCurrentProcess().Id.ToString(),
                InstanceInitiated = DateTime.UtcNow,
                Heartbeat = DateTime.UtcNow,
                SystemProcessType = SystemProcessType.DataImportService
            };
            systemProcess.Id = systemProcess.GenerateInstanceId();

            await repo.Create(systemProcess);

            Assert.True(true);
        }

        [Test]
        [Explicit]
        public async Task Update_InsertsRow_IntoDb()
        {
            var repo = new SystemProcessRepository(new ConnectionStringFactory(_config), _logger);
            var systemProcess = new SystemProcess
            {
                MachineId = Environment.MachineName,
                ProcessId = Process.GetCurrentProcess().Id.ToString(),
                InstanceInitiated = DateTime.UtcNow,
                Heartbeat = DateTime.UtcNow,
                SystemProcessType = SystemProcessType.DataImportService
            };
            systemProcess.Id = systemProcess.GenerateInstanceId();

            await repo.Create(systemProcess);

            systemProcess.Heartbeat = DateTime.UtcNow.AddMinutes(5);

            await repo.Update(systemProcess);

            Assert.True(true);
        }
    }
}
