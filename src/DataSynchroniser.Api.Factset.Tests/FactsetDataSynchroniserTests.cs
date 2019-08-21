namespace DataSynchroniser.Api.Factset.Tests
{
    using System;
    using System.Collections.Generic;

    using DataSynchroniser.Api.Factset.Factset.Interfaces;
    using DataSynchroniser.Api.Factset.Filters.Interfaces;

    using Domain.Core.Financial.Assets;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;

    [TestFixture]
    public class FactsetDataSynchroniserTests
    {
        private IFactsetDataRequestsManager _dataRequestsManager;

        private ILogger<FactsetDataSynchroniser> _logger;

        private ISystemProcessOperationThirdPartyDataRequestContext _requestContext;

        private IFactsetDataRequestFilter _requestFilter;

        [Test]
        public void Constructor_NullLogger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new FactsetDataSynchroniser(this._dataRequestsManager, this._requestFilter, null));
        }

        [Test]
        public void Handle_NonRelevantRequests_DoesNotThrow()
        {
            var synchroniser = this.BuildDataSynchroniser();
            A.CallTo(() => this._requestFilter.ValidAssetType(A<MarketDataRequest>.Ignored)).Returns(false);

            var request = new List<MarketDataRequest> { this.BuildMarketDataRequest("d") };

            Assert.DoesNotThrowAsync(async () => await synchroniser.Handle("a", this._requestContext, request));
        }

        [Test]
        public void Handle_Null_DataRequestContext_DoesNotThrow()
        {
            var synchroniser = this.BuildDataSynchroniser();

            Assert.DoesNotThrowAsync(async () => await synchroniser.Handle("id", null, new MarketDataRequest[0]));
        }

        [Test]
        public void Handle_Null_MarketDataRequests_DoesNotThrow()
        {
            var synchroniser = this.BuildDataSynchroniser();

            Assert.DoesNotThrowAsync(async () => await synchroniser.Handle("id", this._requestContext, null));
        }

        [Test]
        public void Handle_Null_SystemProcessOperationId_DoesNotThrow()
        {
            var synchroniser = this.BuildDataSynchroniser();

            Assert.DoesNotThrowAsync(
                async () => await synchroniser.Handle(null, this._requestContext, new MarketDataRequest[0]));
        }

        [SetUp]
        public void Setup()
        {
            this._dataRequestsManager = A.Fake<IFactsetDataRequestsManager>();

            this._requestFilter = A.Fake<IFactsetDataRequestFilter>();
            A.CallTo(() => this._requestFilter.ValidAssetType(A<MarketDataRequest>.Ignored)).Returns(true);

            this._requestContext = A.Fake<ISystemProcessOperationThirdPartyDataRequestContext>();
            this._logger = A.Fake<ILogger<FactsetDataSynchroniser>>();
        }

        private FactsetDataSynchroniser BuildDataSynchroniser()
        {
            return new FactsetDataSynchroniser(this._dataRequestsManager, this._requestFilter, this._logger);
        }

        private MarketDataRequest BuildMarketDataRequest(string cfi)
        {
            return new MarketDataRequest(
                "a",
                "XLON",
                cfi,
                InstrumentIdentifiers.Null(),
                null,
                null,
                "1",
                true,
                DataSource.Factset);
        }
    }
}