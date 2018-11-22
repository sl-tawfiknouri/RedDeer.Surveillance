using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Interfaces;

namespace Surveillance.DataLayer.Aurora.Analytics
{
    public class RuleAnalyticsAlertsRepository : IRuleAnalyticsAlertsRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger _logger;

        private const string CreateSql = @"
            INSERT INTO RuleAnalyticsAlerts(SystemProcessOperationId, CancelledOrderAlertsRaw, CancelledOrderAlertsAdjusted, HighProfitAlertsRaw, HighProfitAlertsAdjusted, HighVolumeAlertsRaw, HighVolumeAlertsAdjusted, LayeringAlertsRaw, LayeringAlertsAdjusted, MarkingTheCloseAlertsRaw, MarkingTheCloseAlertsAdjusted, SpoofingAlertsRaw, SpoofingAlertsAdjusted, WashTradeAlertsRaw, WashTradeAlertsAdjusted) VALUES(@SystemProcessOperationId, @CancelledOrderAlertsRaw, @CancelledOrderAlertsAdjusted, @HighProfitAlertsRaw, @HighProfitAlertsAdjusted, @HighVolumeAlertsRaw, @HighVolumeAlertsAdjusted, @LayeringAlertsRaw, @LayeringAlertsAdjusted, @MarkingTheCloseAlertsRaw, @MarkingTheCloseAlertsAdjusted, @SpoofingAlertsRaw, @SpoofingAlertsAdjusted, @WashTradeAlertsRaw, @WashTradeAlertsAdjusted);";

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
                return;
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                using (var conn = dbConnection.ExecuteAsync(CreateSql, analytics))
                {
                    await conn;
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
