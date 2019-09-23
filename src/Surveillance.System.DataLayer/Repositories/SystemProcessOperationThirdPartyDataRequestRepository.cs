namespace Surveillance.Auditing.DataLayer.Repositories
{
    using System;
    using System.Threading.Tasks;

    using Dapper;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.DataLayer.Interfaces;
    using Surveillance.Auditing.DataLayer.Processes.Interfaces;
    using Surveillance.Auditing.DataLayer.Repositories.Interfaces;

    public class
        SystemProcessOperationThirdPartyDataRequestRepository : ISystemProcessOperationThirdPartyDataRequestRepository
    {
        private const string CreateSql =
            "INSERT INTO SystemProcessOperationDataSynchroniserRequest(SystemProcessOperationId, QueueMessageId, RuleRunId) VALUES(@SystemProcessOperationId, @QueueMessageId, @RuleRunId); SELECT LAST_INSERT_ID();";

        private readonly IConnectionStringFactory _dbConnectionFactory;

        private readonly object _lock = new object();

        private readonly ILogger<SystemProcessOperationThirdPartyDataRequestRepository> _logger;

        public SystemProcessOperationThirdPartyDataRequestRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<SystemProcessOperationThirdPartyDataRequestRepository> logger)
        {
            this._dbConnectionFactory =
                dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(ISystemProcessOperationThirdPartyDataRequest entity)
        {
            if (entity == null) return;

            lock (this._lock)
            {
                try
                {
                    this._logger.LogInformation($"SystemProcessOperationDistributeRuleRepository SAVING {entity}");
                    using (var dbConnection = this._dbConnectionFactory.BuildConn())
                    using (var conn = dbConnection.QuerySingleAsync<int>(CreateSql, entity))
                    {
                        entity.Id = conn.Result;
                    }
                }
                catch (Exception e)
                {
                    this._logger.LogError(
                        $"System Process Operation Distribute Rule Repository Create Method For {entity.Id} {entity.SystemProcessOperationId}. {e.Message}");
                }
            }
        }
    }
}