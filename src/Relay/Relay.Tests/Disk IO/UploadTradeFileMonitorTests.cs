using System;
using Domain.Trades.Orders;
using Domain.Trades.Streams.Interfaces;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Relay.Configuration.Interfaces;
using Relay.Disk_IO;
using Relay.Disk_IO.Interfaces;
using Utilities.Disk_IO.Interfaces;

namespace Relay.Tests.Disk_IO
{
    [TestFixture]
    public class UploadTradeFileMonitorTests
    {
        private ITradeOrderStream<TradeOrderFrame> _tradeOrderStream;
        private IUploadConfiguration _uploadConfiguration;
        private IReddeerDirectory _directory;
        private IUploadTradeFileProcessor _fileProcessor;
        private ILogger<UploadTradeFileMonitor> _logger;

        [SetUp]
        public void Setup()
        {
            _tradeOrderStream = A.Fake<ITradeOrderStream<TradeOrderFrame>>();
            _uploadConfiguration = A.Fake<IUploadConfiguration>();
            _directory = A.Fake<IReddeerDirectory>();
            _fileProcessor = A.Fake<IUploadTradeFileProcessor>();
            _logger = A.Fake<ILogger<UploadTradeFileMonitor>>();
        }

        [Test]
        public void Constructor_ConsidersNullStream_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new UploadTradeFileMonitor(null, _uploadConfiguration, _directory, _fileProcessor, _logger));
        }

        [Test]
        public void Constructor_ConsidersNullConfiguration_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new UploadTradeFileMonitor(_tradeOrderStream, null, _directory, _fileProcessor, _logger));
        }

        [Test]
        public void Constructor_ConsidersNullDirectory_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new UploadTradeFileMonitor(_tradeOrderStream, _uploadConfiguration, null, _fileProcessor, _logger));
        }

        [Test]
        public void Constructor_ConsidersNullProcessor_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new UploadTradeFileMonitor(_tradeOrderStream, _uploadConfiguration, _directory, null, _logger));
        }

        [Test]
        public void Constructor_ConsidersNullLogger_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new UploadTradeFileMonitor(_tradeOrderStream, _uploadConfiguration, _directory, _fileProcessor, null));
        }

        [Test]
        public void Initiate_EmptyConfigurationPath_Logs()
        {
            var monitor = new UploadTradeFileMonitor(_tradeOrderStream, _uploadConfiguration, _directory, _fileProcessor, _logger);

            monitor.Initiate();

            A
                .CallTo(() => _directory.Create(A<string>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        [Explicit]
        public void Initiate_SetConfigurationPath_Logs()
        {
            var monitor = new UploadTradeFileMonitor(_tradeOrderStream, _uploadConfiguration, _directory, _fileProcessor, _logger);
            A.CallTo(() => _uploadConfiguration.RelayTradeFileUploadDirectoryPath).Returns("testPath");

            monitor.Initiate();

            A
                .CallTo(() => _directory.Create("testPath"))
                .MustHaveHappenedOnceExactly();
        }
    }
}
