namespace DataSynchroniser.Tests.Queues
{
    using System;

    using DataSynchroniser.Queues;

    using Domain.Surveillance.Scheduling.Interfaces;

    using FakeItEasy;

    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
    using Surveillance.DataLayer.Aurora.BMLL.Interfaces;

    [TestFixture]
    public class BmllDataRequestsRescheduleManagerTests
    {
        private IAwsConfiguration _awsConfiguration;

        private IAwsQueueClient _awsQueueClient;

        private IRuleRunDataRequestRepository _dataRequestRepository;

        private ILogger<ScheduleRulePublisher> _logger;

        private IScheduledExecutionMessageBusSerialiser _messageBusSerialiser;

        private ISystemProcessOperationRuleRunRepository _repository;

        [Test]
        public void Ctor_Throws_For_Null_Logger()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new ScheduleRulePublisher(
                    this._awsQueueClient,
                    this._awsConfiguration,
                    this._dataRequestRepository,
                    this._messageBusSerialiser,
                    this._repository,
                    null));
        }

        [Test]
        public void Ctor_Throws_For_Null_Repository()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new ScheduleRulePublisher(
                    this._awsQueueClient,
                    this._awsConfiguration,
                    this._dataRequestRepository,
                    this._messageBusSerialiser,
                    null,
                    this._logger));
        }

        [SetUp]
        public void Setup()
        {
            this._awsQueueClient = A.Fake<IAwsQueueClient>();
            this._awsConfiguration = A.Fake<IAwsConfiguration>();
            this._messageBusSerialiser = A.Fake<IScheduledExecutionMessageBusSerialiser>();

            this._dataRequestRepository = A.Fake<IRuleRunDataRequestRepository>();
            this._repository = A.Fake<ISystemProcessOperationRuleRunRepository>();
            this._logger = A.Fake<ILogger<ScheduleRulePublisher>>();
        }
    }
}