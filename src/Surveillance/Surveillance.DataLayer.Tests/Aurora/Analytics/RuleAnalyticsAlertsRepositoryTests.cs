using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora;
using Surveillance.DataLayer.Aurora.Analytics;
using Surveillance.DataLayer.Configuration;

namespace Surveillance.DataLayer.Tests.Aurora.Analytics
{
    [TestFixture]
    public class RuleAnalyticsAlertsRepositoryTests
    {
        private ILogger<RuleAnalyticsAlertsRepository> _logger;

        [SetUp]
        public void Setup()
        {
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
                FixedIncomeWashTradeAlertsAdjusted = 4
            };

            var config = new DataLayerConfiguration
            {
                AuroraConnectionString = "server=127.0.0.1; port=3306;uid=root;pwd='drunkrabbit101';database=dev_surveillance; Allow User Variables=True"
            };

            var factory = new ConnectionStringFactory(config);
            var repo = new RuleAnalyticsAlertsRepository(factory, _logger);

            await repo.Create(alertAnalytics);
        }
    }
}
