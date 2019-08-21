namespace Surveillance.DataLayer.Aurora.Analytics
{
    using System;
    using System.Threading.Tasks;

    using Dapper;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.Analytics.Interfaces;
    using Surveillance.DataLayer.Aurora.Interfaces;

    public class RuleAnalyticsUniverseRepository : IRuleAnalyticsUniverseRepository
    {
        private const string CreateSql =
            @"INSERT INTO RuleAnalyticsUniverse(SystemProcessOperationId, GenesisEventCount, EschatonEventCount, TradeReddeerCount, TradeReddeerSubmittedCount, StockTickReddeerCount, StockMarketOpenCount, StockMarketCloseCount, UniqueTradersCount, UniqueSecuritiesCount, UniqueMarketsTradedOnCount) VALUES(@SystemProcessOperationId, @GenesisEventCount, @EschatonEventCount, @TradeReddeerCount, @TradeReddeerSubmittedCount, @StockTickReddeerCount, @StockMarketOpenCount, @StockMarketCloseCount, @UniqueTradersCount, @UniqueSecuritiesCount, @UniqueMarketsTradedOnCount)";

        private readonly IConnectionStringFactory _dbConnectionFactory;

        private readonly ILogger _logger;

        public RuleAnalyticsUniverseRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<RuleAnalyticsUniverseRepository> logger)
        {
            this._dbConnectionFactory =
                dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(UniverseAnalytics analytics)
        {
            if (analytics == null)
            {
                this._logger.LogError(
                    "RuleAnalyticsUniverseRepository Create was asked to save a null analytics object. Returning.");
                return;
            }

            var dbConnection = this._dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                this._logger.LogInformation(
                    $"RuleAnalyticsUniverseRepository Create about to save analytics object for system process operation id  {analytics.SystemProcessOperationId}");

                using (var conn = dbConnection.ExecuteAsync(CreateSql, analytics))
                {
                    await conn;

                    this._logger.LogInformation(
                        $"RuleAnalyticsUniverseRepository Create completed saving analytics object for system process operation id  {analytics.SystemProcessOperationId}");
                }
            }
            catch (Exception e)
            {
                this._logger.LogError("RuleAnalyticsUniverseRepository Create method encountered an error!", e);
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }
    }
}