namespace Surveillance.Engine.RuleDistributor.Tests.Queues
{
    using System;

    using Domain.Surveillance.Scheduling.Interfaces;

    using FakeItEasy;

    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.RuleDistributor.Distributor.Interfaces;
    using Surveillance.Engine.RuleDistributor.Queues;

    // ReSharper disable ObjectCreationAsStatement

    /// <summary>
    /// The queue distributed rule subscriber tests.
    /// </summary>
    [TestFixture]
    public class QueueDistributedRuleSubscriberTests
    {
        /// <summary>
        /// The aws client.
        /// </summary>
        private IAwsQueueClient awsClient;

        /// <summary>
        /// The aws configuration.
        /// </summary>
        private IAwsConfiguration awsConfiguration;

        /// <summary>
        /// The bus serialiser.
        /// </summary>
        private IScheduledExecutionMessageBusSerialiser busSerialiser;

        /// <summary>
        /// The schedule disassembler.
        /// </summary>
        private IScheduleDisassembler scheduleDisassembler;

        /// <summary>
        /// The system process context.
        /// </summary>
        private ISystemProcessContext systemProcessContext;

        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<QueueDistributedRuleSubscriber> logger;

        /// <summary>
        /// The constructor considers null aws client throws exception.
        /// </summary>
        [Test]
        public void ConstructorConsidersNullAwsClientThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    new QueueDistributedRuleSubscriber(
                        this.scheduleDisassembler,
                        null,
                        this.awsConfiguration,
                        this.busSerialiser,
                        this.systemProcessContext,
                        this.logger));
        }

        /// <summary>
        /// The constructor considers null aws configuration throws exception.
        /// </summary>
        [Test]
        public void ConstructorConsidersNullAwsConfigurationThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    new QueueDistributedRuleSubscriber(
                        this.scheduleDisassembler,
                        this.awsClient,
                        null,
                        this.busSerialiser,
                        this.systemProcessContext,
                        this.logger));
        }

        /// <summary>
        /// The constructor considers null disassembler throws exception.
        /// </summary>
        [Test]
        public void ConstructorConsidersNullDisassemblerThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    new QueueDistributedRuleSubscriber(
                        null,
                        this.awsClient,
                        this.awsConfiguration,
                        this.busSerialiser,
                        this.systemProcessContext,
                        this.logger));
        }

        /// <summary>
        /// The constructor considers null logger throws exception.
        /// </summary>
        [Test]
        public void ConstructorConsidersNullLoggerThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    new QueueDistributedRuleSubscriber(
                        this.scheduleDisassembler,
                        this.awsClient,
                        this.awsConfiguration,
                        this.busSerialiser,
                        this.systemProcessContext,
                        null));
        }

        /// <summary>
        /// The constructor considers null message bus serializer throws exception.
        /// </summary>
        [Test]
        public void ConstructorConsidersNullMessageBusSerialiserThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    new QueueDistributedRuleSubscriber(
                        this.scheduleDisassembler,
                        this.awsClient,
                        this.awsConfiguration,
                        null,
                        this.systemProcessContext,
                        this.logger));
        }

        /// <summary>
        /// The test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.scheduleDisassembler = A.Fake<IScheduleDisassembler>();
            this.awsClient = A.Fake<IAwsQueueClient>();
            this.awsConfiguration = A.Fake<IAwsConfiguration>();
            this.busSerialiser = A.Fake<IScheduledExecutionMessageBusSerialiser>();
            this.systemProcessContext = A.Fake<ISystemProcessContext>();
            this.logger = A.Fake<ILogger<QueueDistributedRuleSubscriber>>();
        }
    }
}