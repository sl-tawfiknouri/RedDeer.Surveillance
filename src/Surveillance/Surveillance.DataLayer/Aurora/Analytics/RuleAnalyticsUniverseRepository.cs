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
            @"INSERT INTO RuleAnalyticsUniverse(Id, SystemProcessOperationRuleRunId, GenesisEventCount, EschatonEventCount, TradeReddeerCount, TradeReddeerSubmittedCount, StockTickReddeerCount, StockMarketOpenCount, StockMarketCloseCount, UniqueTradersCount, UniqueSecuritiesCount, UniqueMarketsTradedOnCount) VALUES(@Id, @SystemProcessOperationRuleRunId, @GenesisEventCount, @EschatonEventCount, @TradeReddeerCount, @TradeReddeerSubmittedCount, @StockTickReddeerCount, @StockMarketOpenCount, @StockMarketCloseCount, @UniqueTradersCount, @UniqueSecuritiesCount, @UniqueMarketsTradedOnCount)";

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
