using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataSynchroniser.Api.Refinitive;
using DataSynchroniser.Api.Refinitive.Interfaces;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using SharedKernel.Contracts.Markets;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Data.Universe.Refinitiv.Interfaces;

namespace DataSynchroniser.Api.Refinitiv.Tests
{
    public class RefinitivDataSynchroniserTests
    {
        private ITickPriceHistoryServiceClientFactory _tickPriceHistoryServiceClientFactory;
        private IRefinitivTickPriceHistoryApiConfig _refinitivTickPriceHistoryApiConfig;
        private ISystemProcessOperationThirdPartyDataRequestContext _requestContext;
        private ILogger<IRefinitivDataSynchroniser> _logger;

        [SetUp]
        public void Setup()
        {
            _tickPriceHistoryServiceClientFactory = A.Fake<ITickPriceHistoryServiceClientFactory>();
            _refinitivTickPriceHistoryApiConfig = A.Fake<IRefinitivTickPriceHistoryApiConfig>(); 
            _requestContext = A.Fake<ISystemProcessOperationThirdPartyDataRequestContext>();
            _logger = A.Fake<ILogger<IRefinitivDataSynchroniser>>();
        }
        
        [Test]
        public void Constructor_NullFactory_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(
                () => new RefinitivDataSynchroniser(null, this._refinitivTickPriceHistoryApiConfig, this._logger));
        }
        
        [Test]
        public void Constructor_NullConfig_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(
                () => new RefinitivDataSynchroniser(_tickPriceHistoryServiceClientFactory, null, this._logger));
        }
        
        [Test]
        public void Handle_Requests_DoesNotThrow()
        {
            var synchroniser = new RefinitivDataSynchroniser(_tickPriceHistoryServiceClientFactory, _refinitivTickPriceHistoryApiConfig, this._logger);
            IReadOnlyCollection<MarketDataRequest> request = new List<MarketDataRequest>();
            
            Assert.DoesNotThrowAsync(async () => await synchroniser.Handle("a", this._requestContext, request));
        }
        
        [TearDown]
        public void TearDown()
        {
            _tickPriceHistoryServiceClientFactory = null;
            _refinitivTickPriceHistoryApiConfig = null;
            _requestContext = null;
            _logger = null;
        }
    }
}