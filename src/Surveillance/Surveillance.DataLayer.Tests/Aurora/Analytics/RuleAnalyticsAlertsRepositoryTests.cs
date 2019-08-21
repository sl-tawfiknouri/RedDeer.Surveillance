namespace Surveillance.DataLayer.Tests.Aurora.Analytics
{
    using System;
    using System.Threading.Tasks;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.DataLayer.Aurora;
    using Surveillance.DataLayer.Aurora.Analytics;
    using Surveillance.DataLayer.Aurora.Interfaces;
    using Surveillance.DataLayer.Configuration.Interfaces;
    using Surveillance.DataLayer.Tests.Helpers;

    [TestFixture]
    public class RuleAnalyticsAlertsRepositoryTests
    {
        private IDataLayerConfiguration _configuration;

        private IConnectionStringFactory _connectionStringFactory;

        private ILogger<RuleAnalyticsAlertsRepository> _logger;

        [Test]
        public void Ctor_NullConnectionStringFactory_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new RuleAnalyticsAlertsRepository(null, this._logger));
        }

        [Test]
        public void Ctor_NullLogger_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new RuleAnalyticsAlertsRepository(this._connectionStringFactory, null));
        }

        [Test]
        [Explicit]
        public async Task Does_Save_ValidDto()
        {
            var alertAnalytics = new AlertAnalytics
                                     {
                                         SystemProcessOperationId = 1,
                                         CancelledOrderAlertsRaw = 10,
                                         CancelledOrderAlertsAdjusted = 2,
                                         HighProfitAlertsRaw = 3,
                                         HighProfitAlertsAdjusted = 1,
                                         HighVolumeAlertsRaw = 2,
                                         HighVolumeAlertsAdjusted = 1,
                                         LayeringAlertsRaw = 8,
                                         LayeringAlertsAdjusted = 3,
                                         MarkingTheCloseAlertsRaw = 2,
                                         MarkingTheCloseAlertsAdjusted = 2,
                                         SpoofingAlertsRaw = 0,
                                         SpoofingAlertsAdjusted = 0,
                                         WashTradeAlertsRaw = 3,
                                         WashTradeAlertsAdjusted = 1,
                                         FixedIncomeHighProfitAlertsAdjusted = 10,
                                         FixedIncomeHighProfitAlertsRaw = 11,
                                         FixedIncomeHighVolumeIssuanceAlertsAdjusted = 9,
                                         FixedIncomeHighVolumeIssuanceAlertsRaw = 10,
                                         FixedIncomeWashTradeAlertsRaw = 5,
                                         FixedIncomeWashTradeAlertsAdjusted = 4,
                                         RampingAlertsRaw = 99,
                                         RampingAlertsAdjusted = 100,
                                         PlacingOrdersAlertsRaw = 88,
                                         PlacingOrdersAlertsAdjusted = 89
                                     };

            var factory = new ConnectionStringFactory(this._configuration);
            var repo = new RuleAnalyticsAlertsRepository(factory, this._logger);

            await repo.Create(alertAnalytics);
        }

        [Test]
        public void Does_SaveNull_DoesNotThrow()
        {
            var factory = new ConnectionStringFactory(this._configuration);
            var repo = new RuleAnalyticsAlertsRepository(factory, this._logger);

            Assert.DoesNotThrowAsync(() => repo.Create(null));
        }

        [SetUp]
        public void Setup()
        {
            this._configuration = TestHelpers.Config();
            this._connectionStringFactory = A.Fake<IConnectionStringFactory>();
            this._logger = A.Fake<ILogger<RuleAnalyticsAlertsRepository>>();
        }
    }
}