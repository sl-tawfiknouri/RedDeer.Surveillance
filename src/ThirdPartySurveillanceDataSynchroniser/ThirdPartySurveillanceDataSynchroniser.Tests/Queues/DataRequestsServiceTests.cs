using System;
using System.Threading;
using System.Threading.Tasks;
using DataSynchroniser.Manager.Interfaces;
using DataSynchroniser.Queues;
using FakeItEasy;
using Infrastructure.Network.Aws_IO;
using Infrastructure.Network.Aws_IO.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NUnit.Framework;
using SharedKernel.Contracts.Queues;
using SharedKernel.Contracts.Queues.Interfaces;
using Surveillance.Auditing.Context.Interfaces;

namespace DataSynchroniser.Tests.Queues
{
    [TestFixture]
    public class DataRequestsServiceTests
    {
        private ILogger<DataRequestSubscriber> _logger;
        private IAwsQueueClient _awsQueueClient;
        private IAwsConfiguration _awsConfiguration;
        private ISystemProcessContext _sysCtx;
        private IThirdPartyDataRequestSerialiser _serialiser;
        private IDataRequestManager _requestManager;

        [SetUp]
        public void Setup()
        {
            _awsQueueClient = A.Fake<IAwsQueueClient>();
            _awsConfiguration = A.Fake<IAwsConfiguration>();
            _logger = A.Fake<ILogger<DataRequestSubscriber>>();
            _sysCtx = A.Fake<ISystemProcessContext>();
            _serialiser = new ThirdPartyDataRequestSerialiser();
            _requestManager = A.Fake<IDataRequestManager>();
        }

        [Test]
        public void Constructor_NullLogger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DataRequestSubscriber(_awsQueueClient, _awsConfiguration, _sysCtx, _serialiser, _requestManager, null));
        }

        [Test]
        public void Constructor_NullAwsQueueClient_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DataRequestSubscriber(null, _awsConfiguration, _sysCtx, _serialiser, _requestManager, _logger));
        }

        [Test]
        public void Constructor_NullAwsConfiguration_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DataRequestSubscriber(_awsQueueClient, null, _sysCtx, _serialiser, _requestManager, _logger));
        }

        [Test]
        public void Initiate_Calls_AwsQueueClient_SubscribeToQueueAsync()
        {
            var dataRequestsService = new DataRequestSubscriber(_awsQueueClient, _awsConfiguration, _sysCtx, _serialiser, _requestManager, _logger);

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
            var dataRequestsService = new DataRequestSubscriber(_awsQueueClient, _awsConfiguration, _sysCtx, mockSerialiser, _requestManager, _logger);

            await dataRequestsService.Execute("123", "test-str");

            A
                .CallTo(() => mockSerialiser.Deserialise("test-str")).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Execute_Creates_NewOperationContext()
        {
            var dataRequestsService = new DataRequestSubscriber(_awsQueueClient, _awsConfiguration, _sysCtx, _serialiser, _requestManager, _logger);

            await dataRequestsService.Execute("123", "test-str");

            A
                .CallTo(() => _sysCtx.CreateAndStartOperationContext()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Execute_SendsRuleId_ToDataRequestManager()
        {
            var dataRequestsService = new DataRequestSubscriber(_awsQueueClient, _awsConfiguration, _sysCtx, _serialiser, _requestManager, _logger);
            var messageObj = new ThirdPartyDataRequestMessage { SystemProcessOperationId = "123" };
            var msg = JsonConvert.SerializeObject(messageObj);

            await dataRequestsService.Execute("123", msg);

            A
                .CallTo(() => _requestManager.Handle("123", A<ISystemProcessOperationThirdPartyDataRequestContext>.Ignored))
                .MustHaveHappenedOnceExactly();
        }
    }
}
