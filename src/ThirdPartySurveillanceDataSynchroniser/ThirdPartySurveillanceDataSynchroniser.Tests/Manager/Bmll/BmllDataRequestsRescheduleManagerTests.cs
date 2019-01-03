using System;
using DomainV2.Scheduling.Interfaces;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.System.DataLayer.Repositories.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Manager.Bmll;
using Utilities.Aws_IO.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Tests.Manager.Bmll
{
    [TestFixture]
    public class BmllDataRequestsRescheduleManagerTests
    {
        private IAwsQueueClient _awsQueueClient;
        private IAwsConfiguration _awsConfiguration;
        private IScheduledExecutionMessageBusSerialiser _messageBusSerialiser;

        private ISystemProcessOperationRuleRunRepository _repository;
        private ILogger<BmllDataRequestsRescheduleManager> _logger;

        [SetUp]
        public void Setup()
        {
            _awsQueueClient = A.Fake<IAwsQueueClient>();
            _awsConfiguration = A.Fake<IAwsConfiguration>();
            _messageBusSerialiser = A.Fake<IScheduledExecutionMessageBusSerialiser>();

            _repository = A.Fake<ISystemProcessOperationRuleRunRepository>();
            _logger = A.Fake<ILogger<BmllDataRequestsRescheduleManager>>();
        }

        [Test]
        public void Ctor_Throws_For_Null_Logger()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new BmllDataRequestsRescheduleManager(_awsQueueClient, _awsConfiguration, _messageBusSerialiser, _repository, null));
        }

        [Test]
        public void Ctor_Throws_For_Null_Repository()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new BmllDataRequestsRescheduleManager(_awsQueueClient, _awsConfiguration, _messageBusSerialiser, null, _logger));
        }
    }
}
