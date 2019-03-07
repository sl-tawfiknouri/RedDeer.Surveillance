using System;
using DataSynchroniser.Queues;
using Domain.Surveillance.Scheduling.Interfaces;
using FakeItEasy;
using Infrastructure.Network.Aws_IO.Interfaces;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;

namespace DataSynchroniser.Tests.Queues
{
    [TestFixture]
    public class BmllDataRequestsRescheduleManagerTests
    {
        private IAwsQueueClient _awsQueueClient;
        private IAwsConfiguration _awsConfiguration;
        private IScheduledExecutionMessageBusSerialiser _messageBusSerialiser;
        private IRuleRunDataRequestRepository _dataRequestRepository;


        private ISystemProcessOperationRuleRunRepository _repository;
        private ILogger<ScheduleRulePublisher> _logger;

        [SetUp]
        public void Setup()
        {
            _awsQueueClient = A.Fake<IAwsQueueClient>();
            _awsConfiguration = A.Fake<IAwsConfiguration>();
            _messageBusSerialiser = A.Fake<IScheduledExecutionMessageBusSerialiser>();

            _dataRequestRepository = A.Fake<IRuleRunDataRequestRepository>();
            _repository = A.Fake<ISystemProcessOperationRuleRunRepository>();
            _logger = A.Fake<ILogger<ScheduleRulePublisher>>();
        }

        [Test]
        public void Ctor_Throws_For_Null_Logger()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new ScheduleRulePublisher(
                _awsQueueClient,
                _awsConfiguration,
                _dataRequestRepository,
                _messageBusSerialiser,
                _repository,
                null));
        }

        [Test]
        public void Ctor_Throws_For_Null_Repository()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new ScheduleRulePublisher(
                _awsQueueClient,
                _awsConfiguration,
                _dataRequestRepository,
                _messageBusSerialiser,
                null,
                _logger));
        }
    }
}
