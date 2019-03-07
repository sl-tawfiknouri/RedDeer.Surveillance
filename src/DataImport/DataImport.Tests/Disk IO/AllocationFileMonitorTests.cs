using DataImport.Configuration.Interfaces;
using DataImport.Disk_IO.AllocationFile;
using DataImport.Disk_IO.AllocationFile.Interfaces;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using DataImport.MessageBusIO.Interfaces;
using Infrastructure.Network.Disk.Interfaces;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Aurora.Files.Interfaces;
using Surveillance.DataLayer.Aurora.Orders.Interfaces;

namespace DataImport.Tests.Disk_IO
{
    [TestFixture]
    public class AllocationFileMonitorTests
    {
        private IUploadConfiguration _uploadConfiguration;
        private ISystemProcessContext _systemProcessContext;
        private IReddeerDirectory _reddeerDirectory;
        private ILogger<AllocationFileMonitor> _logger;
        private IAllocationFileProcessor _fileProcessor;
        private IOrderAllocationRepository _allocationRepository;
        private IUploadCoordinatorMessageSender _coordinatorMessageSender;
        private IFileUploadOrderAllocationRepository _fileUploadAllocationRepository;

        [SetUp]
        public void Setup()
        {
            _uploadConfiguration = A.Fake<IUploadConfiguration>();
            _systemProcessContext = A.Fake<ISystemProcessContext>();
            _reddeerDirectory = A.Fake<IReddeerDirectory>();

            _fileProcessor = A.Fake<IAllocationFileProcessor>();
            _allocationRepository = A.Fake<IOrderAllocationRepository>();
            _fileUploadAllocationRepository = A.Fake<IFileUploadOrderAllocationRepository>();
            _coordinatorMessageSender = A.Fake<IUploadCoordinatorMessageSender>();
            _logger = A.Fake<ILogger<AllocationFileMonitor>>();
        }

        [Test]
        public void Constructor_ConsidersNull_FileProcessor_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new AllocationFileMonitor(
                    null,
                    _systemProcessContext,
                    _uploadConfiguration,
                    _reddeerDirectory,
                    _allocationRepository,
                    _fileUploadAllocationRepository,
                    _coordinatorMessageSender,
                    _logger));
        }

        [Test]
        public void Constructor_ConsidersNull_Context_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => 
                new AllocationFileMonitor(
                    _fileProcessor,
                    null,
                    _uploadConfiguration,
                    _reddeerDirectory,
                    _allocationRepository,
                    _fileUploadAllocationRepository,
                    _coordinatorMessageSender,
                    _logger));
        }

        [Test]
        public void Constructor_ConsidersNull_Configuration_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new AllocationFileMonitor(
                    _fileProcessor, 
                    _systemProcessContext,
                    null,
                    _reddeerDirectory,
                    _allocationRepository,
                    _fileUploadAllocationRepository,
                    _coordinatorMessageSender,
                    _logger));
        }

        [Test]
        public void Constructor_ConsidersNull_Directory_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new AllocationFileMonitor(
                    _fileProcessor,
                    _systemProcessContext,
                    _uploadConfiguration,
                    null,
                    _allocationRepository,
                    _fileUploadAllocationRepository,
                    _coordinatorMessageSender,
                    _logger));
        }

        [Test]
        public void Constructor_ConsidersNull_AllocationRepository_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new AllocationFileMonitor(
                    _fileProcessor,
                    _systemProcessContext,
                    _uploadConfiguration,
                    _reddeerDirectory,
                    null,
                    _fileUploadAllocationRepository,
                    _coordinatorMessageSender,
                    _logger));
        }

        [Test]
        public void Constructor_ConsidersNull_FileUploadAllocationRepository_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new AllocationFileMonitor(
                    _fileProcessor,
                    _systemProcessContext,
                    _uploadConfiguration,
                    _reddeerDirectory,
                    _allocationRepository,
                    null,
                    _coordinatorMessageSender,
                    _logger));
        }

        [Test]
        public void Constructor_ConsidersNull_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new AllocationFileMonitor(
                    _fileProcessor,
                    _systemProcessContext,
                    _uploadConfiguration,
                    _reddeerDirectory,
                    _allocationRepository,
                    _fileUploadAllocationRepository,
                    _coordinatorMessageSender,
                    null));
        }

        [Test]
        public void ProcessFile_ReturnsFalse_ForEmptyOrNullPath()
        {
            var monitor = BuildAllocationFileMonitor();

            var result = monitor.ProcessFile(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void ProcessFile_ReturnsFalse_APathButNoCsvReads()
        {
            var monitor = BuildAllocationFileMonitor();

            var result = monitor.ProcessFile("a-path");

            Assert.IsFalse(result);
        }

        private AllocationFileMonitor BuildAllocationFileMonitor()
        {
            return new AllocationFileMonitor(
                _fileProcessor,
                _systemProcessContext,
                _uploadConfiguration,
                _reddeerDirectory,
                _allocationRepository,
                _fileUploadAllocationRepository,
                _coordinatorMessageSender,
                _logger);
        }
    }
}
