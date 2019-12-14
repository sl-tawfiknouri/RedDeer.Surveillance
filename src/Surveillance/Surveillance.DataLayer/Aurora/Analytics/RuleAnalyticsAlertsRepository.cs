namespace Surveillance.DataLayer.Aurora.Analytics
{
    using System;
    using System.Threading.Tasks;

    using Dapper;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.Analytics.Interfaces;
    using Surveillance.DataLayer.Aurora.Interfaces;

    public class RuleAnalyticsAlertsRepository : IRuleAnalyticsAlertsRepository
    {
        private const string CreateSql = @"
            INSERT INTO RuleAnalyticsAlerts(SystemProcessOperationId, CancelledOrderAlertsRaw, CancelledOrderAlertsAdjusted, HighProfitAlertsRaw, HighProfitAlertsAdjusted, HighVolumeAlertsRaw, HighVolumeAlertsAdjusted, LayeringAlertsRaw, LayeringAlertsAdjusted, MarkingTheCloseAlertsRaw, MarkingTheCloseAlertsAdjusted, SpoofingAlertsRaw, SpoofingAlertsAdjusted, WashTradeAlertsRaw, WashTradeAlertsAdjusted, FixedIncomeHighProfitAlertsAdjusted, FixedIncomeHighProfitAlertsRaw, FixedIncomeHighVolumeIssuanceAlertsAdjusted, FixedIncomeHighVolumeIssuanceAlertsRaw, FixedIncomeWashTradeAlertsRaw, FixedIncomeWashTradeAlertsAdjusted, PlacingOrdersAlertsRaw, PlacingOrdersAlertsAdjusted, RampingAlertsRaw, RampingAlertsAdjusted) VALUES(@SystemProcessOperationId, @CancelledOrderAlertsRaw, @CancelledOrderAlertsAdjusted, @HighProfitAlertsRaw, @HighProfitAlertsAdjusted, @HighVolumeAlertsRaw, @HighVolumeAlertsAdjusted, @LayeringAlertsRaw, @LayeringAlertsAdjusted, @MarkingTheCloseAlertsRaw, @MarkingTheCloseAlertsAdjusted, @SpoofingAlertsRaw, @SpoofingAlertsAdjusted, @WashTradeAlertsRaw, @WashTradeAlertsAdjusted, @FixedIncomeHighProfitAlertsAdjusted, @FixedIncomeHighProfitAlertsRaw, @FixedIncomeHighVolumeIssuanceAlertsAdjusted, @FixedIncomeHighVolumeIssuanceAlertsRaw, @FixedIncomeWashTradeAlertsRaw, @FixedIncomeWashTradeAlertsAdjusted, @PlacingOrdersAlertsRaw, @PlacingOrdersAlertsAdjusted, @RampingAlertsRaw, @RampingAlertsAdjusted);";

        private readonly IConnectionStringFactory _dbConnectionFactory;

        private readonly ILogger _logger;

        public RuleAnalyticsAlertsRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<RuleAnalyticsAlertsRepository> logger)
        {
            this._dbConnectionFactory =
                dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(AlertAnalytics analytics)
        {
            if (analytics == null)
            {
                this._logger.LogError(
                    "RuleAnalyticsAlertsRepository asked to save a null analytics object. Returning.");
                return;
            }

            var dbConnection = this._dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                this._logger.LogInformation(
                    $"RuleAnalyticsAlertsRepository Create about to save analytics for system process operation id  {analytics.SystemProcessOperationId}");
                using (var conn = dbConnection.ExecuteAsync(CreateSql, analytics))
                {
                    await conn;
                    this._logger.LogInformation(
                        $"RuleAnalyticsAlertsRepository Create has saved analytics for system process operation id  {analytics.SystemProcessOperationId}");
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(e, "RuleAnalyticsAlertsRepository Create method encountered an error!");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }
    }
}