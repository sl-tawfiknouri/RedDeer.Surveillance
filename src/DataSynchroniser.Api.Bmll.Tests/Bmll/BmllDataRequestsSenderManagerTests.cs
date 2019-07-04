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

namespace DataSynchroniser.Api.Bmll.Tests.Bmll
{
    [TestFixture]
    public class BmllDataRequestsSenderManagerTests
    {

        private IBmllDataRequestsGetTimeBars _requestsGetTimeBars;
        private IMarketDataRequestToMinuteBarRequestKeyDtoProjector _marketDataRequestProjector;
        private IBmllTimeBarApi _timeBarRepository;
        private IPolicyFactory _policyFactory;
        private ILogger<BmllDataRequestsApiManager> _logger;

        [SetUp]
        public void Setup()
        {
            _requestsGetTimeBars = A.Fake<IBmllDataRequestsGetTimeBars>();
            _marketDataRequestProjector = A.Fake<IMarketDataRequestToMinuteBarRequestKeyDtoProjector>();
            _timeBarRepository = A.Fake<IBmllTimeBarApi>();
            _policyFactory = A.Fake<IPolicyFactory>();
            _logger = A.Fake<ILogger<BmllDataRequestsApiManager>>();

            A
                .CallTo(() => _timeBarRepository.HeartBeating(A<CancellationToken>.Ignored))
                .Returns(true);
        }

        [Test]
        public void Constructor_Null_GetTimeBars_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new BmllDataRequestsApiManager(null, _marketDataRequestProjector, _policyFactory, _timeBarRepository, _logger));
        }

        [Test]
        public void Constructor_Null_Projector_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new BmllDataRequestsApiManager(_requestsGetTimeBars, null, _policyFactory, _timeBarRepository, _logger));
        }

        [Test]
        public void Constructor_Null_Repository_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new BmllDataRequestsApiManager(_requestsGetTimeBars, _marketDataRequestProjector, _policyFactory, null, _logger));
        }

        [Test]
        public void Constructor_Null_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new BmllDataRequestsApiManager(_requestsGetTimeBars, _marketDataRequestProjector, _policyFactory, _timeBarRepository, null));
        }

        [Test]
        public async Task Send_BmllRequests_Null_Returns_Success()
        {
            var sender = BuildSenderManager();

            var result = await sender.Send(null, true);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsEmpty(result.Value);
        }

        [Test]
        public async Task Send_BmllRequests_ProjectRequestToKeys_Null_Returns_Success()
        {
            var sender = BuildSenderManager();
            var bmllRequests = new List<MarketDataRequest>
            {
                MarketDataRequest.Null()
            };

            A
                .CallTo(() => _marketDataRequestProjector.ProjectToRequestKeys(A<List<MarketDataRequest>>.Ignored))
                .Returns(null);

            var result = await sender.Send(bmllRequests, true);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsEmpty(result.Value);
        }

        [Test]
        public async Task Send_BmllRequests_OkProjectedKeys_Returns_Success()
        {
            var sender = BuildSenderManager();
            var bmllRequests = new List<MarketDataRequest>
            {
                MarketDataRequest.Null()
            };

            var bmllRequestDtos = new List<MinuteBarRequestKeyDto>
            {
                new MinuteBarRequestKeyDto("a-figi", "1min", new DateTime(2019, 01, 01))
            };
            
            A
                .CallTo(() => _marketDataRequestProjector.ProjectToRequestKeys(A<List<MarketDataRequest>>.Ignored))
                .Returns(bmllRequestDtos);

            A
                .CallTo(() => _timeBarRepository.StatusMinuteBars(A<GetMinuteBarRequestStatusesRequest>.Ignored))
                .Returns(BmllStatusMinuteBarResult.Completed);

            var result = await sender.Send(bmllRequests, true);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsEmpty(result.Value);
        }

        private BmllDataRequestsApiManager BuildSenderManager()
        {
            return new BmllDataRequestsApiManager(_requestsGetTimeBars, _marketDataRequestProjector, _policyFactory, _timeBarRepository, _logger);
        }
    }
}
