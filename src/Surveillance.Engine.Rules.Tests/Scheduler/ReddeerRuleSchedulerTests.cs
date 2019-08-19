namespace Surveillance.Engine.Rules.Tests.Scheduler
{
    using System;

    using Domain.Surveillance.Scheduling.Interfaces;

    using FakeItEasy;

    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analysis.Interfaces;
    using Surveillance.Engine.Rules.Queues;
    using Surveillance.Engine.Rules.Utility.Interfaces;

    [TestFixture]
    public class ReddeerRuleSchedulerTests
    {
        private IAnalysisEngine _analysisEngine;

        private IApiHeartbeat _apiHeartbeat;

        private IAwsConfiguration _awsConfiguration;

        private IAwsQueueClient _awsQueueClient;

        private ISystemProcessContext _ctx;

        private ILogger<QueueRuleSubscriber> _logger;

        private IScheduledExecutionMessageBusSerialiser _messageBusSerialiser;

        [Test]
        public void Constructor_NullAnalysisEngine_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(
                () =>

                    // ReSharper disable once ObjectCreationAsStatement
                    new QueueRuleSubscriber(
                        null,
                        this._awsQueueClient,
                        this._awsConfiguration,
                        this._messageBusSerialiser,
                        this._apiHeartbeat,
                        this._ctx,
                        this._logger));
        }

        [Test]
        public void Constructor_NullApiHeartbeat_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(
                () =>

                    // ReSharper disable once ObjectCreationAsStatement
                    new QueueRuleSubscriber(
                        this._analysisEngine,
                        this._awsQueueClient,
                        this._awsConfiguration,
                        this._messageBusSerialiser,
                        null,
                        this._ctx,
                        this._logger));
        }

        [Test]
        public void Constructor_NullConfiguration_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(
                () =>

                    // ReSharper disable once ObjectCreationAsStatement
                    new QueueRuleSubscriber(
                        this._analysisEngine,
                        this._awsQueueClient,
                        null,
                        this._messageBusSerialiser,
                        this._apiHeartbeat,
                        this._ctx,
                        this._logger));
        }

        [Test]
        public void Constructor_NullCtx_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(
                () =>

                    // ReSharper disable once ObjectCreationAsStatement
                    new QueueRuleSubscriber(
                        this._analysisEngine,
                        this._awsQueueClient,
                        this._awsConfiguration,
                        this._messageBusSerialiser,
                        this._apiHeartbeat,
                        null,
                        this._logger));
        }

        [Test]
        public void Constructor_NullLogger_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(
                () =>

                    // ReSharper disable once ObjectCreationAsStatement
                    new QueueRuleSubscriber(
                        this._analysisEngine,
                        this._awsQueueClient,
                        this._awsConfiguration,
                        this._messageBusSerialiser,
                        this._apiHeartbeat,
                        this._ctx,
                        null));
        }

        [Test]
        public void Constructor_NullMessageBusSerialiser_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(
                () =>

                    // ReSharper disable once ObjectCreationAsStatement
                    new QueueRuleSubscriber(
                        this._analysisEngine,
                        this._awsQueueClient,
                        this._awsConfiguration,
                        null,
                        this._apiHeartbeat,
                        this._ctx,
                        this._logger));
        }

        [Test]
        public void Constructor_NullQueueClient_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(
                () =>

                    // ReSharper disable once ObjectCreationAsStatement
                    new QueueRuleSubscriber(
                        this._analysisEngine,
                        null,
                        this._awsConfiguration,
                        this._messageBusSerialiser,
                        this._apiHeartbeat,
                        this._ctx,
                        this._logger));
        }

        [SetUp]
        public void Setup()
        {
            this._awsQueueClient = A.Fake<IAwsQueueClient>();
            this._awsConfiguration = A.Fake<IAwsConfiguration>();
            this._messageBusSerialiser = A.Fake<IScheduledExecutionMessageBusSerialiser>();
            this._apiHeartbeat = A.Fake<IApiHeartbeat>();
            this._ctx = A.Fake<ISystemProcessContext>();

            this._analysisEngine = A.Fake<IAnalysisEngine>();
            this._logger = A.Fake<ILogger<QueueRuleSubscriber>>();
        }
    }
}