using System;
using DomainV2.Scheduling.Interfaces;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.RuleDistributor.Distributor.Interfaces;
using Surveillance.Engine.RuleDistributor.Queues;
using Utilities.Aws_IO.Interfaces;

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
        private ILogger<QueueReddeerDistributedRuleSubscriber> _logger;

        [SetUp]
        public void Setup()
        {
            _scheduleDisassembler = A.Fake<IScheduleDisassembler>();
            _awsClient = A.Fake<IAwsQueueClient>();
            _awsConfiguration = A.Fake<IAwsConfiguration>();
            _busSerialiser = A.Fake<IScheduledExecutionMessageBusSerialiser>();
            _systemProcessContext = A.Fake<ISystemProcessContext>();
            _logger = A.Fake<ILogger<QueueReddeerDistributedRuleSubscriber>>();
        }

        [Test]
        public void Constructor_ConsidersNullDisassembler_ToBeExceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new QueueReddeerDistributedRuleSubscriber(
                    null,
                    _awsClient,
                    _awsConfiguration,
                    _busSerialiser,
                    _systemProcessContext,
                    _logger));
        }

        [Test]
        public void Constructor_ConsidersNullAwsClient_ToBeExceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new QueueReddeerDistributedRuleSubscriber(
                    _scheduleDisassembler,
                    null,
                    _awsConfiguration,
                    _busSerialiser,
                    _systemProcessContext,
                    _logger));
        }

        [Test]
        public void Constructor_ConsidersNullAwsConfiguration_ToBeExceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new QueueReddeerDistributedRuleSubscriber(
                    _scheduleDisassembler,
                    _awsClient,
                    null,
                    _busSerialiser,
                    _systemProcessContext,
                    _logger));
        }


        [Test]
        public void Constructor_ConsidersNullMessageBusSerialiser_ToBeExceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new QueueReddeerDistributedRuleSubscriber(
                    _scheduleDisassembler,
                    _awsClient,
                    _awsConfiguration,
                    null,
                    _systemProcessContext,
                    _logger));
        }

        [Test]
        public void Constructor_ConsidersNullLogger_ToBeExceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new QueueReddeerDistributedRuleSubscriber(
                    _scheduleDisassembler,
                    _awsClient,
                    _awsConfiguration,
                    _busSerialiser,
                    _systemProcessContext,
                    null));
        }
    }
}
