namespace DataImport.Tests.Disk_IO
{
    using System;
    using System.Collections.Generic;

    using DataImport.Configuration.Interfaces;
    using DataImport.Disk_IO.TradeFile;
    using DataImport.Disk_IO.TradeFile.Interfaces;
    using DataImport.MessageBusIO.Interfaces;
    using DataImport.Services.Interfaces;

    using Domain.Core.Trading.Orders;

    using FakeItEasy;

    using Infrastructure.Network.Disk.Interfaces;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using SharedKernel.Files.Orders.Interfaces;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.DataLayer.Aurora.Files.Interfaces;
    using Surveillance.DataLayer.Aurora.Orders.Interfaces;

    [TestFixture]
    public class UploadTradeFileMonitorTests
    {
        private IReddeerDirectory _directory;

        private IEnrichmentService _enrichmentService;

        private IUploadTradeFileProcessor _fileProcessor;

        private IFileUploadOrdersRepository _fileUploadOrdersRepository;

        private ILogger<UploadTradeFileMonitor> _logger;

        private IUploadCoordinatorMessageSender _messageSender;

        private IOmsVersioner _omsVersioner;

        private IOrdersRepository _ordersRepository;

        private ISystemProcessContext _systemProcessContext;

        private IUploadConfiguration _uploadConfiguration;

        [Test]
        public void Constructor_ConsidersNullConfiguration_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new UploadTradeFileMonitor(
                    null,
                    this._directory,
                    this._fileProcessor,
                    this._enrichmentService,
                    this._ordersRepository,
                    this._fileUploadOrdersRepository,
                    this._messageSender,
                    this._systemProcessContext,
                    this._omsVersioner,
                    this._logger));
        }

        [Test]
        public void Constructor_ConsidersNullDirectory_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new UploadTradeFileMonitor(
                    this._uploadConfiguration,
                    null,
                    this._fileProcessor,
                    this._enrichmentService,
                    this._ordersRepository,
                    this._fileUploadOrdersRepository,
                    this._messageSender,
                    this._systemProcessContext,
                    this._omsVersioner,
                    this._logger));
        }

        [Test]
        public void Constructor_ConsidersNullLogger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new UploadTradeFileMonitor(
                    this._uploadConfiguration,
                    this._directory,
                    this._fileProcessor,
                    this._enrichmentService,
                    this._ordersRepository,
                    this._fileUploadOrdersRepository,
                    this._messageSender,
                    this._systemProcessContext,
                    this._omsVersioner,
                    null));
        }

        [Test]
        public void Constructor_ConsidersNullProcessor_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new UploadTradeFileMonitor(
                    this._uploadConfiguration,
                    this._directory,
                    null,
                    this._enrichmentService,
                    this._ordersRepository,
                    this._fileUploadOrdersRepository,
                    this._messageSender,
                    this._systemProcessContext,
                    this._omsVersioner,
                    this._logger));
        }

        [Test]
        public void Initiate_EmptyConfigurationPath_Logs()
        {
            var monitor = new UploadTradeFileMonitor(
                this._uploadConfiguration,
                this._directory,
                this._fileProcessor,
                this._enrichmentService,
                this._ordersRepository,
                this._fileUploadOrdersRepository,
                this._messageSender,
                this._systemProcessContext,
                this._omsVersioner,
                this._logger);

            monitor.Initiate();

            A.CallTo(() => this._directory.Create(A<string>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        [Explicit]
        public void Initiate_SetConfigurationPath_Logs()
        {
            var monitor = new UploadTradeFileMonitor(
                this._uploadConfiguration,
                this._directory,
                this._fileProcessor,
                this._enrichmentService,
                this._ordersRepository,
                this._fileUploadOrdersRepository,
                this._messageSender,
                this._systemProcessContext,
                this._omsVersioner,
                this._logger);

            A.CallTo(() => this._uploadConfiguration.DataImportTradeFileUploadDirectoryPath).Returns("testPath");

            monitor.Initiate();

            A.CallTo(() => this._directory.Create("testPath")).MustHaveHappenedOnceExactly();
        }

        [SetUp]
        public void Setup()
        {
            this._uploadConfiguration = A.Fake<IUploadConfiguration>();
            this._directory = A.Fake<IReddeerDirectory>();
            this._fileProcessor = A.Fake<IUploadTradeFileProcessor>();
            this._enrichmentService = A.Fake<IEnrichmentService>();
            this._ordersRepository = A.Fake<IOrdersRepository>();
            this._fileUploadOrdersRepository = A.Fake<IFileUploadOrdersRepository>();
            this._systemProcessContext = A.Fake<ISystemProcessContext>();
            this._messageSender = A.Fake<IUploadCoordinatorMessageSender>();
            this._omsVersioner = A.Fake<IOmsVersioner>();
            this._logger = A.Fake<ILogger<UploadTradeFileMonitor>>();

            A.CallTo(() => this._omsVersioner.ProjectOmsVersion(A<IReadOnlyCollection<Order>>.Ignored))
                .ReturnsLazily(a => (IReadOnlyCollection<Order>)a.Arguments[0]);
        }
    }
}