using System;
using DomainV2.Scheduling.Interfaces;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Systems.Auditing.Context.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.Tests.Scheduler
{
    [TestFixture]
    public class ReddeerRuleSchedulerTests
    {
        private IApiHeartbeat _apiHeartbeat;
        private ISystemProcessContext _ctx;
        private IAwsQueueClient _awsQueueClient;
        private IAwsConfiguration _awsConfiguration;
        private IScheduledExecutionMessageBusSerialiser _messageBusSerialiser;

        private IAnalysisEngine _analysisEngine;
        private ILogger<ReddeerRuleScheduler> _logger;

        [SetUp]
        public void Setup()
        {
            _awsQueueClient = A.Fake<IAwsQueueClient>();
            _awsConfiguration = A.Fake<IAwsConfiguration>();
            _messageBusSerialiser = A.Fake<IScheduledExecutionMessageBusSerialiser>();
            _apiHeartbeat = A.Fake<IApiHeartbeat>();
            _ctx = A.Fake<ISystemProcessContext>();

            _analysisEngine = A.Fake<IAnalysisEngine>();
            _logger = A.Fake <ILogger<ReddeerRuleScheduler>>();
        }

        [Test]
        public void Constructor_NullAnalysisEngine_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new ReddeerRuleScheduler(
                    null,
                    _awsQueueClient,
                    _awsConfiguration, 
                    _messageBusSerialiser,
                    _apiHeartbeat,
                    _ctx,
                    _logger));
        }

        [Test]
        public void Constructor_NullQueueClient_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new ReddeerRuleScheduler(
                    _analysisEngine,
                    null,
                    _awsConfiguration,
                    _messageBusSerialiser,
                    _apiHeartbeat,
                    _ctx,
                    _logger));
        }

        [Test]
        public void Constructor_NullConfiguration_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new ReddeerRuleScheduler(
                    _analysisEngine,
                    _awsQueueClient,
                    null,
                    _messageBusSerialiser,
                    _apiHeartbeat,
                    _ctx,
                    _logger));
        }

        [Test]
        public void Constructor_NullMessageBusSerialiser_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new ReddeerRuleScheduler(
                    _analysisEngine,
                    _awsQueueClient,
                    _awsConfiguration,
                    null,
                    _apiHeartbeat,
                    _ctx,
                    _logger));
        }

        [Test]
        public void Constructor_NullApiHeartbeat_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new ReddeerRuleScheduler(
                    _analysisEngine,
                    _awsQueueClient,
                    _awsConfiguration,
                    _messageBusSerialiser,
                    null,
                    _ctx,
                    _logger));
        }

        [Test]
        public void Constructor_NullCtx_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new ReddeerRuleScheduler(
                    _analysisEngine,
                    _awsQueueClient,
                    _awsConfiguration,
                    _messageBusSerialiser,
                    _apiHeartbeat,
                    null,
                    _logger));
        }

        [Test]
        public void Constructor_NullLogger_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new ReddeerRuleScheduler(
                    _analysisEngine,
                    _awsQueueClient,
                    _awsConfiguration,
                    _messageBusSerialiser,
                    _apiHeartbeat,
                    _ctx,
                    null));
        }
    }
}
