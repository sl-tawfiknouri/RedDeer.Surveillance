using System;
using DataImport.Configuration.Interfaces;
using DataImport.Disk_IO.TradeFile;
using DataImport.Disk_IO.TradeFile.Interfaces;
using DomainV2.Streams.Interfaces;
using DomainV2.Trading;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.System.Auditing.Context.Interfaces;
using Utilities.Disk_IO.Interfaces;

namespace DataImport.Tests.Disk_IO
{
    [TestFixture]
    public class UploadTradeFileMonitorTests
    {
        private IOrderStream<Order> _tradeOrderStream;
        private IUploadConfiguration _uploadConfiguration;
        private IReddeerDirectory _directory;
        private IUploadTradeFileProcessor _fileProcessor;
        private ISystemProcessContext _systemProcessContext;
        private ILogger<UploadTradeFileMonitor> _logger;

        [SetUp]
        public void Setup()
        {
            _tradeOrderStream = A.Fake<IOrderStream<Order>>();
            _uploadConfiguration = A.Fake<IUploadConfiguration>();
            _directory = A.Fake<IReddeerDirectory>();
            _fileProcessor = A.Fake<IUploadTradeFileProcessor>();
            _systemProcessContext = A.Fake<ISystemProcessContext>();
            _logger = A.Fake<ILogger<UploadTradeFileMonitor>>();
        }

        [Test]
        public void Constructor_ConsidersNullStream_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new UploadTradeFileMonitor(null, _uploadConfiguration, _directory, _fileProcessor, _systemProcessContext, _logger));
        }

        [Test]
        public void Constructor_ConsidersNullConfiguration_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new UploadTradeFileMonitor(_tradeOrderStream, null, _directory, _fileProcessor, _systemProcessContext, _logger));
        }

        [Test]
        public void Constructor_ConsidersNullDirectory_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new UploadTradeFileMonitor(_tradeOrderStream, _uploadConfiguration, null, _fileProcessor, _systemProcessContext, _logger));
        }

        [Test]
        public void Constructor_ConsidersNullProcessor_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new UploadTradeFileMonitor(_tradeOrderStream, _uploadConfiguration, _directory, null, _systemProcessContext, _logger));
        }

        [Test]
        public void Constructor_ConsidersNullLogger_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new UploadTradeFileMonitor(_tradeOrderStream, _uploadConfiguration, _directory, _fileProcessor, _systemProcessContext, null));
        }

        [Test]
        public void Initiate_EmptyConfigurationPath_Logs()
        {
            var monitor = new UploadTradeFileMonitor(_tradeOrderStream, _uploadConfiguration, _directory, _fileProcessor, _systemProcessContext, _logger);

            monitor.Initiate();

            A
                .CallTo(() => _directory.Create(A<string>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        [Explicit]
        public void Initiate_SetConfigurationPath_Logs()
        {
            var monitor = new UploadTradeFileMonitor(_tradeOrderStream, _uploadConfiguration, _directory, _fileProcessor, _systemProcessContext, _logger);
            A.CallTo(() => _uploadConfiguration.RelayTradeFileUploadDirectoryPath).Returns("testPath");

            monitor.Initiate();

            A
                .CallTo(() => _directory.Create("testPath"))
                .MustHaveHappenedOnceExactly();
        }
    }
}
