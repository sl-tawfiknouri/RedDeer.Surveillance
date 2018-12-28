using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
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

        [SetUp]
        public void Setup()
        {
            _awsQueueClient = A.Fake<IAwsQueueClient>();
            _awsConfiguration = A.Fake<IAwsConfiguration>();
            _logger = A.Fake<ILogger<DataRequestsService>>();
        }

        [Test]
        public void Constructor_NullLogger_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DataRequestsService(_awsQueueClient, _awsConfiguration, null));
        }

        [Test]
        public void Constructor_NullAwsQueueClient_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DataRequestsService(null, _awsConfiguration, _logger));
        }

        [Test]
        public void Constructor_NullAwsConfiguration_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DataRequestsService(_awsQueueClient, null, _logger));
        }

        [Test]
        public void Initiate_Calls_AwsQueueClient_SubscribeToQueueAsync()
        {
            var dataRequestsService = new DataRequestsService(_awsQueueClient, _awsConfiguration, _logger);

            dataRequestsService.Initiate();

            A
                .CallTo(() => _awsQueueClient.SubscribeToQueueAsync(A<string>.Ignored,
                A<Func<string, string, Task>>.Ignored, A<CancellationToken>.Ignored,
                A<AwsResusableCancellationToken>.Ignored))
                .MustHaveHappenedOnceExactly();
        }


    }
}
