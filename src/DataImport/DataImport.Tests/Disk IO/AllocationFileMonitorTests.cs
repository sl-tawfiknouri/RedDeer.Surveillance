namespace DataImport.Tests.Disk_IO
{
    using System;

    using DataImport.Configuration.Interfaces;
    using DataImport.Disk_IO.AllocationFile;
    using DataImport.Disk_IO.AllocationFile.Interfaces;
    using DataImport.MessageBusIO.Interfaces;

    using FakeItEasy;

    using Infrastructure.Network.Disk.Interfaces;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.DataLayer.Aurora.Files.Interfaces;
    using Surveillance.DataLayer.Aurora.Orders.Interfaces;

    [TestFixture]
    public class AllocationFileMonitorTests
    {
        private IOrderAllocationRepository _allocationRepository;

        private IUploadCoordinatorMessageSender _coordinatorMessageSender;

        private IAllocationFileProcessor _fileProcessor;

        private IFileUploadOrderAllocationRepository _fileUploadAllocationRepository;

        private ILogger<AllocationFileMonitor> _logger;

        private IReddeerDirectory _reddeerDirectory;

        private ISystemProcessContext _systemProcessContext;

        private IUploadConfiguration _uploadConfiguration;

        [Test]
        public void Constructor_ConsidersNull_AllocationRepository_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new AllocationFileMonitor(
                    this._fileProcessor,
                    this._systemProcessContext,
                    this._uploadConfiguration,
                    this._reddeerDirectory,
                    null,
                    this._fileUploadAllocationRepository,
                    this._coordinatorMessageSender,
                    this._logger));
        }

        [Test]
        public void Constructor_ConsidersNull_Configuration_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new AllocationFileMonitor(
                    this._fileProcessor,
                    this._systemProcessContext,
                    null,
                    this._reddeerDirectory,
                    this._allocationRepository,
                    this._fileUploadAllocationRepository,
                    this._coordinatorMessageSender,
                    this._logger));
        }

        [Test]
        public void Constructor_ConsidersNull_Context_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new AllocationFileMonitor(
                    this._fileProcessor,
                    null,
                    this._uploadConfiguration,
                    this._reddeerDirectory,
                    this._allocationRepository,
                    this._fileUploadAllocationRepository,
                    this._coordinatorMessageSender,
                    this._logger));
        }

        [Test]
        public void Constructor_ConsidersNull_Directory_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new AllocationFileMonitor(
                    this._fileProcessor,
                    this._systemProcessContext,
                    this._uploadConfiguration,
                    null,
                    this._allocationRepository,
                    this._fileUploadAllocationRepository,
                    this._coordinatorMessageSender,
                    this._logger));
        }

        [Test]
        public void Constructor_ConsidersNull_FileProcessor_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new AllocationFileMonitor(
                    null,
                    this._systemProcessContext,
                    this._uploadConfiguration,
                    this._reddeerDirectory,
                    this._allocationRepository,
                    this._fileUploadAllocationRepository,
                    this._coordinatorMessageSender,
                    this._logger));
        }

        [Test]
        public void Constructor_ConsidersNull_FileUploadAllocationRepository_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new AllocationFileMonitor(
                    this._fileProcessor,
                    this._systemProcessContext,
                    this._uploadConfiguration,
                    this._reddeerDirectory,
                    this._allocationRepository,
                    null,
                    this._coordinatorMessageSender,
                    this._logger));
        }

        [Test]
        public void Constructor_ConsidersNull_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new AllocationFileMonitor(
                    this._fileProcessor,
                    this._systemProcessContext,
                    this._uploadConfiguration,
                    this._reddeerDirectory,
                    this._allocationRepository,
                    this._fileUploadAllocationRepository,
                    this._coordinatorMessageSender,
                    null));
        }

        [Test]
        public void ProcessFile_ReturnsFalse_APathButNoCsvReads()
        {
            var monitor = this.BuildAllocationFileMonitor();

            var result = monitor.ProcessFile("a-path");

            Assert.IsFalse(result);
        }

        [Test]
        public void ProcessFile_ReturnsFalse_ForEmptyOrNullPath()
        {
            var monitor = this.BuildAllocationFileMonitor();

            var result = monitor.ProcessFile(null);

            Assert.IsFalse(result);
        }

        [SetUp]
        public void Setup()
        {
            this._uploadConfiguration = A.Fake<IUploadConfiguration>();
            this._systemProcessContext = A.Fake<ISystemProcessContext>();
            this._reddeerDirectory = A.Fake<IReddeerDirectory>();

            this._fileProcessor = A.Fake<IAllocationFileProcessor>();
            this._allocationRepository = A.Fake<IOrderAllocationRepository>();
            this._fileUploadAllocationRepository = A.Fake<IFileUploadOrderAllocationRepository>();
            this._coordinatorMessageSender = A.Fake<IUploadCoordinatorMessageSender>();
            this._logger = A.Fake<ILogger<AllocationFileMonitor>>();
        }

        private AllocationFileMonitor BuildAllocationFileMonitor()
        {
            return new AllocationFileMonitor(
                this._fileProcessor,
                this._systemProcessContext,
                this._uploadConfiguration,
                this._reddeerDirectory,
                this._allocationRepository,
                this._fileUploadAllocationRepository,
                this._coordinatorMessageSender,
                this._logger);
        }
    }
}