using DataImport.Configuration.Interfaces;
using DataImport.Disk_IO.AllocationFile;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.System.Auditing.Context.Interfaces;
using System;
using Utilities.Disk_IO.Interfaces;

namespace DataImport.Tests.Disk_IO
{
    [TestFixture]
    public class AllocationFileMonitorTests
    {
        private IUploadConfiguration _uploadConfiguration;
        private ISystemProcessContext _systemProcessContext;
        private IReddeerDirectory _reddeerDirectory;
        private ILogger<AllocationFileMonitor> _logger;


        [SetUp]
        public void Setup()
        {
            _uploadConfiguration = A.Fake<IUploadConfiguration>();
            _systemProcessContext = A.Fake<ISystemProcessContext>();
            _reddeerDirectory = A.Fake<IReddeerDirectory>();
            _logger = A.Fake<ILogger<AllocationFileMonitor>>();
        }

        [Test]
        public void Constructor_ConsidersNull_Context_ToBeExceptional()
        {
            Assert.Throws<ArgumentNullException>(() => new AllocationFileMonitor(null, _uploadConfiguration, _reddeerDirectory, _logger));
        }

        [Test]
        public void Constructor_ConsidersNull_Configuration_ToBeExceptional()
        {
            Assert.Throws<ArgumentNullException>(() => new AllocationFileMonitor(_systemProcessContext, null, _reddeerDirectory, _logger));
        }

        [Test]
        public void ProcessFile_ReturnsFalse_ForEmptyOrNullPath()
        {
            var monitor = new AllocationFileMonitor(_systemProcessContext, _uploadConfiguration, _reddeerDirectory, _logger);

            var result = monitor.ProcessFile(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void ProcessFile_ReturnsTrue_ForEmptyOrNullPath()
        {
            var monitor = new AllocationFileMonitor(_systemProcessContext, _uploadConfiguration, _reddeerDirectory, _logger);

            var result = monitor.ProcessFile("a-path");

            Assert.IsTrue(result);
        }

        
    }
}
