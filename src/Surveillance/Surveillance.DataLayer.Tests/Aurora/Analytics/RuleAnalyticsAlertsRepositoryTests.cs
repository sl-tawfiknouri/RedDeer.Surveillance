using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora;
using Surveillance.DataLayer.Aurora.Analytics;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.DataLayer.Tests.Helpers;

namespace Surveillance.DataLayer.Tests.Aurora.Analytics
{
    [TestFixture]
    public class RuleAnalyticsAlertsRepositoryTests
    {
        private IDataLayerConfiguration _configuration;
        private ILogger<RuleAnalyticsAlertsRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _configuration = TestHelpers.Config();
            _logger = A.Fake<ILogger<RuleAnalyticsAlertsRepository>>();
        }

        [Test]
        [Explicit]
        public async Task Does_Save()
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

            var factory = new ConnectionStringFactory(_configuration);
            var repo = new RuleAnalyticsAlertsRepository(factory, _logger);

            await repo.Create(alertAnalytics);
        }
    }
}
