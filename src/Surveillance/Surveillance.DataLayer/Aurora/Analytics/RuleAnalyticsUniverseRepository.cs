using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Analytics.Interfaces;
using Surveillance.DataLayer.Aurora.Interfaces;

namespace Surveillance.DataLayer.Aurora.Analytics
{
    public class RuleAnalyticsUniverseRepository : IRuleAnalyticsUniverseRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger _logger;

        private const string CreateSql =
            @"INSERT INTO RuleAnalyticsUniverse(SystemProcessOperationId, GenesisEventCount, EschatonEventCount, TradeReddeerCount, TradeReddeerSubmittedCount, StockTickReddeerCount, StockMarketOpenCount, StockMarketCloseCount, UniqueTradersCount, UniqueSecuritiesCount, UniqueMarketsTradedOnCount) VALUES(@SystemProcessOperationId, @GenesisEventCount, @EschatonEventCount, @TradeReddeerCount, @TradeReddeerSubmittedCount, @StockTickReddeerCount, @StockMarketOpenCount, @StockMarketCloseCount, @UniqueTradersCount, @UniqueSecuritiesCount, @UniqueMarketsTradedOnCount)";

        public RuleAnalyticsUniverseRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<RuleAnalyticsUniverseRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(UniverseAnalytics analytics)
        {
            if (analytics == null)
            {
                _logger.LogError($"RuleAnalyticsUniverseRepository Create was asked to save a null analytics object. Returning.");
                return;
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                _logger.LogInformation($"RuleAnalyticsUniverseRepository Create about to save analytics object for system process operation id  {analytics.SystemProcessOperationId}");

                using (var conn = dbConnection.ExecuteAsync(CreateSql, analytics))
                {
                    await conn;

                    _logger.LogInformation($"RuleAnalyticsUniverseRepository Create completed saving analytics object for system process operation id  {analytics.SystemProcessOperationId}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"RuleAnalyticsUniverseRepository Create method encountered an error!", e);
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }
    }
}
