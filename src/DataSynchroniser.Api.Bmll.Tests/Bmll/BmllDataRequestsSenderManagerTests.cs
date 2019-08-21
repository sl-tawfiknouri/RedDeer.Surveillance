namespace DataSynchroniser.Api.Bmll.Tests.Bmll
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using DataSynchroniser.Api.Bmll.Bmll;
    using DataSynchroniser.Api.Bmll.Bmll.Interfaces;

    using FakeItEasy;

    using Firefly.Service.Data.BMLL.Shared.Dtos;
    using Firefly.Service.Data.BMLL.Shared.Requests;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using PollyFacade.Policies.Interfaces;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Reddeer.ApiClient.BmllMarketData;
    using Surveillance.Reddeer.ApiClient.BmllMarketData.Interfaces;

    [TestFixture]
    public class BmllDataRequestsSenderManagerTests
    {
        private ILogger<BmllDataRequestsApiManager> _logger;

        private IMarketDataRequestToMinuteBarRequestKeyDtoProjector _marketDataRequestProjector;

        private IPolicyFactory _policyFactory;

        private IBmllDataRequestsGetTimeBars _requestsGetTimeBars;

        private IBmllTimeBarApi _timeBarRepository;

        [Test]
        public void Constructor_Null_GetTimeBars_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new BmllDataRequestsApiManager(
                    null,
                    this._marketDataRequestProjector,
                    this._policyFactory,
                    this._timeBarRepository,
                    this._logger));
        }

        [Test]
        public void Constructor_Null_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new BmllDataRequestsApiManager(
                    this._requestsGetTimeBars,
                    this._marketDataRequestProjector,
                    this._policyFactory,
                    this._timeBarRepository,
                    null));
        }

        [Test]
        public void Constructor_Null_Projector_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new BmllDataRequestsApiManager(
                    this._requestsGetTimeBars,
                    null,
                    this._policyFactory,
                    this._timeBarRepository,
                    this._logger));
        }

        [Test]
        public void Constructor_Null_Repository_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new BmllDataRequestsApiManager(
                    this._requestsGetTimeBars,
                    this._marketDataRequestProjector,
                    this._policyFactory,
                    null,
                    this._logger));
        }

        [Test]
        public async Task Send_BmllRequests_Null_Returns_Success()
        {
            var sender = this.BuildSenderManager();

            var result = await sender.Send(null, true);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsEmpty(result.Value);
        }

        [Test]
        public async Task Send_BmllRequests_OkProjectedKeys_Returns_Success()
        {
            var sender = this.BuildSenderManager();
            var bmllRequests = new List<MarketDataRequest> { MarketDataRequest.Null() };

            var bmllRequestDtos = new List<MinuteBarRequestKeyDto>
                                      {
                                          new MinuteBarRequestKeyDto("a-figi", "1min", new DateTime(2019, 01, 01))
                                      };

            A.CallTo(() => this._marketDataRequestProjector.ProjectToRequestKeys(A<List<MarketDataRequest>>.Ignored))
                .Returns(bmllRequestDtos);

            A.CallTo(() => this._timeBarRepository.StatusMinuteBars(A<GetMinuteBarRequestStatusesRequest>.Ignored))
                .Returns(BmllStatusMinuteBarResult.Completed);

            var result = await sender.Send(bmllRequests, true);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsEmpty(result.Value);
        }

        [Test]
        public async Task Send_BmllRequests_ProjectRequestToKeys_Null_Returns_Success()
        {
            var sender = this.BuildSenderManager();
            var bmllRequests = new List<MarketDataRequest> { MarketDataRequest.Null() };

            A.CallTo(() => this._marketDataRequestProjector.ProjectToRequestKeys(A<List<MarketDataRequest>>.Ignored))
                .Returns(null);

            var result = await sender.Send(bmllRequests, true);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsEmpty(result.Value);
        }

        [SetUp]
        public void Setup()
        {
            this._requestsGetTimeBars = A.Fake<IBmllDataRequestsGetTimeBars>();
            this._marketDataRequestProjector = A.Fake<IMarketDataRequestToMinuteBarRequestKeyDtoProjector>();
            this._timeBarRepository = A.Fake<IBmllTimeBarApi>();
            this._policyFactory = A.Fake<IPolicyFactory>();
            this._logger = A.Fake<ILogger<BmllDataRequestsApiManager>>();

            A.CallTo(() => this._timeBarRepository.HeartBeating(A<CancellationToken>.Ignored)).Returns(true);
        }

        private BmllDataRequestsApiManager BuildSenderManager()
        {
            return new BmllDataRequestsApiManager(
                this._requestsGetTimeBars,
                this._marketDataRequestProjector,
                this._policyFactory,
                this._timeBarRepository,
                this._logger);
        }
    }
}