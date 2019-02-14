using System;
using DataImport.Configuration.Interfaces;
using DataImport.Disk_IO.TradeFile;
using DataImport.Disk_IO.TradeFile.Interfaces;
using DataImport.Services.Interfaces;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Aurora.Trade.Interfaces;
using Utilities.Disk_IO.Interfaces;

namespace DataImport.Tests.Disk_IO
{
    [TestFixture]
    public class UploadTradeFileMonitorTests
    {
        private IUploadConfiguration _uploadConfiguration;
        private IReddeerDirectory _directory;
        private IUploadTradeFileProcessor _fileProcessor;
        private IEnrichmentService _enrichmentService;
        private IOrdersRepository _ordersRepository;

        private ISystemProcessContext _systemProcessContext;
        private ILogger<UploadTradeFileMonitor> _logger;

        [SetUp]
        public void Setup()
        {
            _uploadConfiguration = A.Fake<IUploadConfiguration>();
            _directory = A.Fake<IReddeerDirectory>();
            _fileProcessor = A.Fake<IUploadTradeFileProcessor>();
            _enrichmentService = A.Fake<IEnrichmentService>();
            _ordersRepository = A.Fake<IOrdersRepository>();
            _systemProcessContext = A.Fake<ISystemProcessContext>();
            _logger = A.Fake<ILogger<UploadTradeFileMonitor>>();
        }

        [Test]
        public void Constructor_ConsidersNullConfiguration_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new UploadTradeFileMonitor(null, _directory, _fileProcessor, _enrichmentService, _ordersRepository, _systemProcessContext, _logger));
        }

        [Test]
        public void Constructor_ConsidersNullDirectory_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new UploadTradeFileMonitor( _uploadConfiguration, null, _fileProcessor, _enrichmentService, _ordersRepository, _systemProcessContext, _logger));
        }

        [Test]
        public void Constructor_ConsidersNullProcessor_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new UploadTradeFileMonitor(_uploadConfiguration, _directory, null, _enrichmentService, _ordersRepository, _systemProcessContext, _logger));
        }

        [Test]
        public void Constructor_ConsidersNullLogger_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new UploadTradeFileMonitor(_uploadConfiguration, _directory, _fileProcessor, _enrichmentService, _ordersRepository, _systemProcessContext, null));
        }

        [Test]
        public void Initiate_EmptyConfigurationPath_Logs()
        {
            var monitor = new UploadTradeFileMonitor(_uploadConfiguration, _directory, _fileProcessor, _enrichmentService, _ordersRepository, _systemProcessContext, _logger);

            monitor.Initiate();

            A
                .CallTo(() => _directory.Create(A<string>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        [Explicit]
        public void Initiate_SetConfigurationPath_Logs()
        {
            var monitor = new UploadTradeFileMonitor(_uploadConfiguration, _directory, _fileProcessor, _enrichmentService, _ordersRepository, _systemProcessContext, _logger);
            A.CallTo(() => _uploadConfiguration.DataImportTradeFileUploadDirectoryPath).Returns("testPath");

            monitor.Initiate();

            A
                .CallTo(() => _directory.Create("testPath"))
                .MustHaveHappenedOnceExactly();
        }
    }
}
