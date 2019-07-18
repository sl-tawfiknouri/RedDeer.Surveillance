using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.DataLayer.Interfaces;
using Surveillance.Auditing.DataLayer.Processes.Interfaces;
using Surveillance.Auditing.DataLayer.Repositories.Interfaces;

namespace Surveillance.Auditing.DataLayer.Repositories
{
    public class SystemProcessOperationThirdPartyDataRequestRepository : ISystemProcessOperationThirdPartyDataRequestRepository
    {
        private readonly object _lock = new object();
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

            lock (_lock)
            {
                try
                {
                    _logger.LogInformation($"SystemProcessOperationDistributeRuleRepository SAVING {entity}");
                    using (var dbConnection = _dbConnectionFactory.BuildConn())
                    using (var conn = dbConnection.QuerySingleAsync<int>(CreateSql, entity))
                    {
                        entity.Id = conn.Result;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"System Process Operation Distribute Rule Repository Create Method For {entity.Id} {entity.SystemProcessOperationId}. {e.Message}");
                }
            }
        }
    }
}
