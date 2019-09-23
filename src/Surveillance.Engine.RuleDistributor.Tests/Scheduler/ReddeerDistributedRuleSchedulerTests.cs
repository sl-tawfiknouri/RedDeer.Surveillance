namespace Surveillance.Engine.RuleDistributor.Tests.Scheduler
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

    [TestFixture]
    public class ReddeerDistributedRuleSchedulerTests
    {
        private IAwsQueueClient _awsClient;

        private IAwsConfiguration _awsConfiguration;

        private IScheduledExecutionMessageBusSerialiser _busSerialiser;

        private ILogger<QueueDistributedRuleSubscriber> _logger;

        private IScheduleDisassembler _scheduleDisassembler;

        private ISystemProcessContext _systemProcessContext;

        [Test]
        public void Constructor_ConsidersNullAwsClient_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(
                () =>

                    // ReSharper disable once ObjectCreationAsStatement
                    new QueueDistributedRuleSubscriber(
                        this._scheduleDisassembler,
                        null,
                        this._awsConfiguration,
                        this._busSerialiser,
                        this._systemProcessContext,
                        this._logger));
        }

        [Test]
        public void Constructor_ConsidersNullAwsConfiguration_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(
                () =>

                    // ReSharper disable once ObjectCreationAsStatement
                    new QueueDistributedRuleSubscriber(
                        this._scheduleDisassembler,
                        this._awsClient,
                        null,
                        this._busSerialiser,
                        this._systemProcessContext,
                        this._logger));
        }

        [Test]
        public void Constructor_ConsidersNullDisassembler_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(
                () =>

                    // ReSharper disable once ObjectCreationAsStatement
                    new QueueDistributedRuleSubscriber(
                        null,
                        this._awsClient,
                        this._awsConfiguration,
                        this._busSerialiser,
                        this._systemProcessContext,
                        this._logger));
        }

        [Test]
        public void Constructor_ConsidersNullLogger_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(
                () =>

                    // ReSharper disable once ObjectCreationAsStatement
                    new QueueDistributedRuleSubscriber(
                        this._scheduleDisassembler,
                        this._awsClient,
                        this._awsConfiguration,
                        this._busSerialiser,
                        this._systemProcessContext,
                        null));
        }

        [Test]
        public void Constructor_ConsidersNullMessageBusSerialiser_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(
                () =>

                    // ReSharper disable once ObjectCreationAsStatement
                    new QueueDistributedRuleSubscriber(
                        this._scheduleDisassembler,
                        this._awsClient,
                        this._awsConfiguration,
                        null,
                        this._systemProcessContext,
                        this._logger));
        }

        [SetUp]
        public void Setup()
        {
            this._scheduleDisassembler = A.Fake<IScheduleDisassembler>();
            this._awsClient = A.Fake<IAwsQueueClient>();
            this._awsConfiguration = A.Fake<IAwsConfiguration>();
            this._busSerialiser = A.Fake<IScheduledExecutionMessageBusSerialiser>();
            this._systemProcessContext = A.Fake<ISystemProcessContext>();
            this._logger = A.Fake<ILogger<QueueDistributedRuleSubscriber>>();
        }
    }
}