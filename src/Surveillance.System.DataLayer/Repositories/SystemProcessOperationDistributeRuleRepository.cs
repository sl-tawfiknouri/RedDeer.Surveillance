using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Surveillance.System.DataLayer.Interfaces;
using Surveillance.System.DataLayer.Processes.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.DataLayer.Repositories
{
    public class SystemProcessOperationDistributeRuleRepository : ISystemProcessOperationDistributeRuleRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<ISystemProcessOperationDistributeRuleRepository> _logger;

        private const string CreateSql = "INSERT INTO SystemProcessOperationDistributeRule(SystemProcessOperationId, ScheduleRuleInitialStart, ScheduleRuleInitialEnd, RulesDistributed) VALUES(@SystemProcessOperationId, @ScheduleRuleInitialStart, @ScheduleRuleInitialEnd, @RulesDistributed); SELECT LAST_INSERT_ID();";
        private const string UpdateSql = "UPDATE SystemProcessOperationDistributeRule SET ScheduleRuleInitialStart = @ScheduleRuleInitialStart, ScheduleRuleInitialEnd = @ScheduleRuleInitialEnd, RulesDistributed = @RulesDistributed WHERE Id = @Id";

        public SystemProcessOperationDistributeRuleRepository(
            IConnectionStringFactory connectionFactory,
            ILogger<ISystemProcessOperationDistributeRuleRepository> logger)
        {
            _dbConnectionFactory =
                connectionFactory
                ?? throw new ArgumentNullException(nameof(connectionFactory));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task Create(ISystemProcessOperationDistributeRule entity)
        {
            if (entity == null)
            {
                return;
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                using (var conn = dbConnection.QuerySingleAsync<int>(CreateSql, entity))
                {
                    entity.Id = await conn;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"System Process Operation Distribute Rule Repository Create Method For {entity.Id} {entity.SystemProcessOperationId}. {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        public async Task Update(ISystemProcessOperationDistributeRule entity)
        {
            if (entity == null)
            {
                return;
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                using (var conn = dbConnection.ExecuteAsync(UpdateSql, entity))
                {
                    await conn;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"System Process Operation Distribute Rule Repository Update Method For {entity.Id} {entity.SystemProcessOperationId}. {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }
    }
}
