using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Surveillance.System.DataLayer.Interfaces;
using Surveillance.System.DataLayer.Processes.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.DataLayer.Repositories
{
    public class SystemProcessOperationThirdPartyDataRequestRepository : ISystemProcessOperationThirdPartyDataRequestRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<SystemProcessOperationThirdPartyDataRequestRepository> _logger;

        private const string CreateSql = "INSERT INTO SystemProcessOperationDataSynchroniserRequest(SystemProcessOperationId, QueueMessageId, RuleRunId) VALUES(@SystemProcessOperationId, @QueueMessageId, @RuleRunId); SELECT LAST_INSERT_ID();";

        public SystemProcessOperationThirdPartyDataRequestRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<SystemProcessOperationThirdPartyDataRequestRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(ISystemProcessOperationThirdPartyDataRequest entity)
        {
            if (entity == null)
            {
                return;
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                _logger.LogInformation($"SystemProcessOperationDistributeRuleRepository SAVING {entity}");
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
    }
}
