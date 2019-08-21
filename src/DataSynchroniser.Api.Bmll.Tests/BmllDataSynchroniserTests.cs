namespace DataSynchroniser.Api.Bmll.Tests
{
    using System;

    using DataSynchroniser.Api.Bmll.Bmll.Interfaces;
    using DataSynchroniser.Api.Bmll.Filters;
    using DataSynchroniser.Api.Bmll.Filters.Interfaces;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;

    [TestFixture]
    public class BmllDataSynchroniserTests
    {
        private IBmllDataRequestManager _dataRequestManager;

        private IBmllDataRequestFilter _filter;

        private ILogger<BmllDataSynchroniser> _logger;

        private ISystemProcessOperationThirdPartyDataRequestContext _requestContext;

        [Test]
        public void Constructor_Filter_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new BmllDataSynchroniser(this._dataRequestManager, null, this._logger));
        }

        [Test]
        public void Constructor_Logger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new BmllDataSynchroniser(this._dataRequestManager, this._filter, null));
        }

        [Test]
        public void Handle_DataRequestContext_Null_DoesNotThrow()
        {
            var dataSynchroniser = this.BuildDataSynchroniser();

            Assert.DoesNotThrowAsync(async () => await dataSynchroniser.Handle("a", null, new MarketDataRequest[0]));
        }

        [Test]
        public void Handle_MarketDataRequests_Null_DoesNotThrow()
        {
            var dataSynchroniser = this.BuildDataSynchroniser();

            Assert.DoesNotThrowAsync(async () => await dataSynchroniser.Handle("b", this._requestContext, null));
        }

        [Test]
        public void Handle_SystemProcessOperationId_Null_DoesNotThrow()
        {
            var dataSynchroniser = this.BuildDataSynchroniser();

            Assert.DoesNotThrowAsync(
                async () => await dataSynchroniser.Handle(null, this._requestContext, new MarketDataRequest[0]));
        }

        [SetUp]
        public void Setup()
        {
            this._dataRequestManager = A.Fake<IBmllDataRequestManager>();
            this._filter = new BmllDataRequestFilter();
            this._requestContext = A.Fake<ISystemProcessOperationThirdPartyDataRequestContext>();
            this._logger = A.Fake<ILogger<BmllDataSynchroniser>>();
        }

        private BmllDataSynchroniser BuildDataSynchroniser()
        {
            return new BmllDataSynchroniser(this._dataRequestManager, this._filter, this._logger);
        }
    }
}