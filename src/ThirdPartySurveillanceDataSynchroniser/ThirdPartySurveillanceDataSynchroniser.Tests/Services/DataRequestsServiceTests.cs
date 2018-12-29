﻿using System;
using System.Threading;
using System.Threading.Tasks;
using DomainV2.DTO;
using DomainV2.DTO.Interfaces;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.System.Auditing.Context.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Services;
using Utilities.Aws_IO;
using Utilities.Aws_IO.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Tests.Services
{
    [TestFixture]
    public class DataRequestsServiceTests
    {
        private ILogger<DataRequestsService> _logger;
        private IAwsQueueClient _awsQueueClient;
        private IAwsConfiguration _awsConfiguration;
        private ISystemProcessContext _sysCtx;
        private IThirdPartyDataRequestSerialiser _serialiser;

        [SetUp]
        public void Setup()
        {
            _awsQueueClient = A.Fake<IAwsQueueClient>();
            _awsConfiguration = A.Fake<IAwsConfiguration>();
            _logger = A.Fake<ILogger<DataRequestsService>>();
            _sysCtx = A.Fake<ISystemProcessContext>();
            _serialiser = new ThirdPartyDataRequestSerialiser();
        }

        [Test]
        public void Constructor_NullLogger_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DataRequestsService(_awsQueueClient, _awsConfiguration, _sysCtx, _serialiser, null));
        }

        [Test]
        public void Constructor_NullAwsQueueClient_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DataRequestsService(null, _awsConfiguration, _sysCtx, _serialiser, _logger));
        }

        [Test]
        public void Constructor_NullAwsConfiguration_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DataRequestsService(_awsQueueClient, null, _sysCtx, _serialiser, _logger));
        }

        [Test]
        public void Initiate_Calls_AwsQueueClient_SubscribeToQueueAsync()
        {
            var dataRequestsService = new DataRequestsService(_awsQueueClient, _awsConfiguration, _sysCtx, _serialiser, _logger);

            dataRequestsService.Initiate();

            A
                .CallTo(() => _awsQueueClient.SubscribeToQueueAsync(A<string>.Ignored,
                A<Func<string, string, Task>>.Ignored, A<CancellationToken>.Ignored,
                A<AwsResusableCancellationToken>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Execute_Deserialises_Message()
        {
            var mockSerialiser = A.Fake<IThirdPartyDataRequestSerialiser>();
            var dataRequestsService = new DataRequestsService(_awsQueueClient, _awsConfiguration, _sysCtx, mockSerialiser, _logger);

            await dataRequestsService.Execute("123", "test-str");

            A
                .CallTo(() => mockSerialiser.Deserialise("test-str")).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Execute_Creates_NewOperationContext()
        {
            var dataRequestsService = new DataRequestsService(_awsQueueClient, _awsConfiguration, _sysCtx, _serialiser, _logger);

            await dataRequestsService.Execute("123", "test-str");

            A
                .CallTo(() => _sysCtx.CreateAndStartOperationContext()).MustHaveHappenedOnceExactly();
        }

    }
}
