namespace DataSynchroniser.Tests.Queues
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using DataSynchroniser.Manager.Interfaces;
    using DataSynchroniser.Queues;

    using FakeItEasy;

    using Infrastructure.Network.Aws;
    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using NUnit.Framework;

    using SharedKernel.Contracts.Queues;
    using SharedKernel.Contracts.Queues.Interfaces;

    using Surveillance.Auditing.Context.Interfaces;

    [TestFixture]
    public class DataRequestsServiceTests
    {
        private IAwsConfiguration _awsConfiguration;

        private IAwsQueueClient _awsQueueClient;

        private ILogger<DataRequestSubscriber> _logger;

        private IDataRequestManager _requestManager;

        private IThirdPartyDataRequestSerialiser _serialiser;

        private ISystemProcessContext _sysCtx;

        [Test]
        public void Constructor_NullAwsConfiguration_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new DataRequestSubscriber(
                    this._awsQueueClient,
                    null,
                    this._sysCtx,
                    this._serialiser,
                    this._requestManager,
                    this._logger));
        }

        [Test]
        public void Constructor_NullAwsQueueClient_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new DataRequestSubscriber(
                    null,
                    this._awsConfiguration,
                    this._sysCtx,
                    this._serialiser,
                    this._requestManager,
                    this._logger));
        }

        [Test]
        public void Constructor_NullLogger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new DataRequestSubscriber(
                    this._awsQueueClient,
                    this._awsConfiguration,
                    this._sysCtx,
                    this._serialiser,
                    this._requestManager,
                    null));
        }

        [Test]
        public async Task Execute_Creates_NewOperationContext()
        {
            var dataRequestsService = new DataRequestSubscriber(
                this._awsQueueClient,
                this._awsConfiguration,
                this._sysCtx,
                this._serialiser,
                this._requestManager,
                this._logger);

            await dataRequestsService.Execute("123", "test-str");

            A.CallTo(() => this._sysCtx.CreateAndStartOperationContext()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Execute_Deserialises_Message()
        {
            var mockSerialiser = A.Fake<IThirdPartyDataRequestSerialiser>();
            var dataRequestsService = new DataRequestSubscriber(
                this._awsQueueClient,
                this._awsConfiguration,
                this._sysCtx,
                mockSerialiser,
                this._requestManager,
                this._logger);

            await dataRequestsService.Execute("123", "test-str");

            A.CallTo(() => mockSerialiser.Deserialise("test-str")).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Execute_SendsRuleId_ToDataRequestManager()
        {
            var dataRequestsService = new DataRequestSubscriber(
                this._awsQueueClient,
                this._awsConfiguration,
                this._sysCtx,
                this._serialiser,
                this._requestManager,
                this._logger);
            var messageObj = new ThirdPartyDataRequestMessage { SystemProcessOperationId = "123" };
            var msg = JsonConvert.SerializeObject(messageObj);

            await dataRequestsService.Execute("123", msg);

            A.CallTo(
                () => this._requestManager.Handle(
                    "123",
                    A<ISystemProcessOperationThirdPartyDataRequestContext>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Initiate_Calls_AwsQueueClient_SubscribeToQueueAsync()
        {
            var dataRequestsService = new DataRequestSubscriber(
                this._awsQueueClient,
                this._awsConfiguration,
                this._sysCtx,
                this._serialiser,
                this._requestManager,
                this._logger);

            dataRequestsService.Initiate();

            A.CallTo(
                () => this._awsQueueClient.SubscribeToQueueAsync(
                    A<string>.Ignored,
                    A<Func<string, string, Task>>.Ignored,
                    A<CancellationToken>.Ignored,
                    A<AwsResusableCancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [SetUp]
        public void Setup()
        {
            this._awsQueueClient = A.Fake<IAwsQueueClient>();
            this._awsConfiguration = A.Fake<IAwsConfiguration>();
            this._logger = A.Fake<ILogger<DataRequestSubscriber>>();
            this._sysCtx = A.Fake<ISystemProcessContext>();
            this._serialiser = new ThirdPartyDataRequestSerialiser();
            this._requestManager = A.Fake<IDataRequestManager>();
        }
    }
}