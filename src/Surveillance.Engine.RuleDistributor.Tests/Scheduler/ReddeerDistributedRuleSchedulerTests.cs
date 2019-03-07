using System;
using Domain.Surveillance.Scheduling.Interfaces;
using FakeItEasy;
using Infrastructure.Network.Aws_IO.Interfaces;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.RuleDistributor.Distributor.Interfaces;
using Surveillance.Engine.RuleDistributor.Queues;

namespace Surveillance.Engine.RuleDistributor.Tests.Scheduler
{
    [TestFixture]
    public class ReddeerDistributedRuleSchedulerTests
    {
        private IScheduleDisassembler _scheduleDisassembler;
        private IAwsQueueClient _awsClient;
        private IAwsConfiguration _awsConfiguration;
        private IScheduledExecutionMessageBusSerialiser _busSerialiser;
        private ISystemProcessContext _systemProcessContext;
        private ILogger<QueueDistributedRuleSubscriber> _logger;

        [SetUp]
        public void Setup()
        {
            _scheduleDisassembler = A.Fake<IScheduleDisassembler>();
            _awsClient = A.Fake<IAwsQueueClient>();
            _awsConfiguration = A.Fake<IAwsConfiguration>();
            _busSerialiser = A.Fake<IScheduledExecutionMessageBusSerialiser>();
            _systemProcessContext = A.Fake<ISystemProcessContext>();
            _logger = A.Fake<ILogger<QueueDistributedRuleSubscriber>>();
        }

        [Test]
        public void Constructor_ConsidersNullDisassembler_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new QueueDistributedRuleSubscriber(
                    null,
                    _awsClient,
                    _awsConfiguration,
                    _busSerialiser,
                    _systemProcessContext,
                    _logger));
        }

        [Test]
        public void Constructor_ConsidersNullAwsClient_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new QueueDistributedRuleSubscriber(
                    _scheduleDisassembler,
                    null,
                    _awsConfiguration,
                    _busSerialiser,
                    _systemProcessContext,
                    _logger));
        }

        [Test]
        public void Constructor_ConsidersNullAwsConfiguration_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new QueueDistributedRuleSubscriber(
                    _scheduleDisassembler,
                    _awsClient,
                    null,
                    _busSerialiser,
                    _systemProcessContext,
                    _logger));
        }


        [Test]
        public void Constructor_ConsidersNullMessageBusSerialiser_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new QueueDistributedRuleSubscriber(
                    _scheduleDisassembler,
                    _awsClient,
                    _awsConfiguration,
                    null,
                    _systemProcessContext,
                    _logger));
        }

        [Test]
        public void Constructor_ConsidersNullLogger_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new QueueDistributedRuleSubscriber(
                    _scheduleDisassembler,
                    _awsClient,
                    _awsConfiguration,
                    _busSerialiser,
                    _systemProcessContext,
                    null));
        }
    }
}
