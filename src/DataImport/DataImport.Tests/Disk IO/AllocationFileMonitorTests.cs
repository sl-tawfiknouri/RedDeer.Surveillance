using DataImport.Configuration.Interfaces;
using DataImport.Disk_IO.AllocationFile;
using DataImport.Disk_IO.AllocationFile.Interfaces;
using DomainV2.Streams.Interfaces;
using DomainV2.Trading;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Systems.Auditing.Context.Interfaces;
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
        private IOrderAllocationStream<OrderAllocation> _orderAllocationStream;
        private IAllocationFileProcessor _fileProcessor;


        [SetUp]
        public void Setup()
        {
            _uploadConfiguration = A.Fake<IUploadConfiguration>();
            _systemProcessContext = A.Fake<ISystemProcessContext>();
            _reddeerDirectory = A.Fake<IReddeerDirectory>();
            _orderAllocationStream = A.Fake<IOrderAllocationStream<OrderAllocation>>();
            _fileProcessor = A.Fake<IAllocationFileProcessor>();
            _logger = A.Fake<ILogger<AllocationFileMonitor>>();
        }

        [Test]
        public void Constructor_ConsidersNull_Context_ToBeExceptional()
        {
            Assert.Throws<ArgumentNullException>(() => new AllocationFileMonitor(_orderAllocationStream, _fileProcessor, null, _uploadConfiguration, _reddeerDirectory, _logger));
        }

        [Test]
        public void Constructor_ConsidersNull_Configuration_ToBeExceptional()
        {
            Assert.Throws<ArgumentNullException>(() => new AllocationFileMonitor(_orderAllocationStream, _fileProcessor, _systemProcessContext, null, _reddeerDirectory, _logger));
        }

        [Test]
        public void ProcessFile_ReturnsFalse_ForEmptyOrNullPath()
        {
            var monitor = Build();

            var result = monitor.ProcessFile(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void ProcessFile_ReturnsFalse_APathButNoCsvReads()
        {
            var monitor = Build();


            var result = monitor.ProcessFile("a-path");

            Assert.IsFalse(result);
        }

        private AllocationFileMonitor Build()
        {
            return new AllocationFileMonitor(
                _orderAllocationStream,
                _fileProcessor,
                _systemProcessContext,
                _uploadConfiguration,
                _reddeerDirectory,
                _logger);
        }
    }
}
