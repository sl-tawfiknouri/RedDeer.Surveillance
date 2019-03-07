using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Analytics.Interfaces;
using Surveillance.DataLayer.Aurora.Interfaces;

namespace Surveillance.DataLayer.Aurora.Analytics
{
    public class RuleAnalyticsAlertsRepository : IRuleAnalyticsAlertsRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger _logger;

        private const string CreateSql = @"
            INSERT INTO RuleAnalyticsAlerts(SystemProcessOperationId, CancelledOrderAlertsRaw, CancelledOrderAlertsAdjusted, HighProfitAlertsRaw, HighProfitAlertsAdjusted, HighVolumeAlertsRaw, HighVolumeAlertsAdjusted, LayeringAlertsRaw, LayeringAlertsAdjusted, MarkingTheCloseAlertsRaw, MarkingTheCloseAlertsAdjusted, SpoofingAlertsRaw, SpoofingAlertsAdjusted, WashTradeAlertsRaw, WashTradeAlertsAdjusted, FixedIncomeHighProfitAlertsAdjusted, FixedIncomeHighProfitAlertsRaw, FixedIncomeHighVolumeIssuanceAlertsAdjusted, FixedIncomeHighVolumeIssuanceAlertsRaw, FixedIncomeWashTradeAlertsRaw, FixedIncomeWashTradeAlertsAdjusted) VALUES(@SystemProcessOperationId, @CancelledOrderAlertsRaw, @CancelledOrderAlertsAdjusted, @HighProfitAlertsRaw, @HighProfitAlertsAdjusted, @HighVolumeAlertsRaw, @HighVolumeAlertsAdjusted, @LayeringAlertsRaw, @LayeringAlertsAdjusted, @MarkingTheCloseAlertsRaw, @MarkingTheCloseAlertsAdjusted, @SpoofingAlertsRaw, @SpoofingAlertsAdjusted, @WashTradeAlertsRaw, @WashTradeAlertsAdjusted, @FixedIncomeHighProfitAlertsAdjusted, @FixedIncomeHighProfitAlertsRaw, @FixedIncomeHighVolumeIssuanceAlertsAdjusted, @FixedIncomeHighVolumeIssuanceAlertsRaw, @FixedIncomeWashTradeAlertsRaw, @FixedIncomeWashTradeAlertsAdjusted);";

        public RuleAnalyticsAlertsRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<RuleAnalyticsAlertsRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(AlertAnalytics analytics)
        {
            if (analytics == null)
            {
                _logger.LogError($"RuleAnalyticsAlertsRepository asked to save a null analytics object. Returning.");
                return;
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                _logger.LogInformation($"RuleAnalyticsAlertsRepository Create about to save analytics for system process operation id  {analytics.SystemProcessOperationId}");
                using (var conn = dbConnection.ExecuteAsync(CreateSql, analytics))
                {
                    await conn;
                    _logger.LogInformation($"RuleAnalyticsAlertsRepository Create has saved analytics for system process operation id  {analytics.SystemProcessOperationId}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"RuleAnalyticsAlertsRepository Create method encountered an error!", e);
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }
    }
}
