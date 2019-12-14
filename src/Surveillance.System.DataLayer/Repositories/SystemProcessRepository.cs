namespace Surveillance.Auditing.DataLayer.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Dapper;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.DataLayer.Interfaces;
    using Surveillance.Auditing.DataLayer.Processes;
    using Surveillance.Auditing.DataLayer.Processes.Interfaces;
    using Surveillance.Auditing.DataLayer.Repositories.Interfaces;

    public class SystemProcessRepository : ISystemProcessRepository
    {
        private const string CreateSql =
            "INSERT INTO SystemProcess(Id, InstanceInitiated, Heartbeat, MachineId, ProcessId, SystemProcessTypeId, CancelRuleQueueDeletedFlag) VALUES(@Id, @InstanceInitiated, @Heartbeat, @MachineId, @ProcessId, @SystemProcessType, @CancelRuleQueueDeletedFlag);";

        private const string GetDashboardSql =
            "SELECT Id, InstanceInitiated, Heartbeat, MachineId, ProcessId, SystemProcessTypeId as SystemProcessType, CancelRuleQueueDeletedFlag FROM SystemProcess ORDER BY Heartbeat DESC LIMIT 10;";

        private const string GetExpiredProcessesSql =
            @"SELECT Id, InstanceInitiated, Heartbeat, MachineId, ProcessId, SystemProcessTypeId as SystemProcessType, CancelRuleQueueDeletedFlag 
            FROM SystemProcess
            WHERE CancelRuleQueueDeletedFlag = 0
            AND Heartbeat < @CancelParamDate;";

        private const string UpdateSql =
            "UPDATE SystemProcess SET Heartbeat = @Heartbeat, CancelRuleQueueDeletedFlag = @CancelRuleQueueDeletedFlag WHERE Id = @Id";

        private readonly IConnectionStringFactory _dbConnectionFactory;

        private readonly ILogger _logger;

        public SystemProcessRepository(
            IConnectionStringFactory connectionStringFactory,
            ILogger<ISystemProcessRepository> logger)
        {
            this._dbConnectionFactory = connectionStringFactory
                                        ?? throw new ArgumentNullException(nameof(connectionStringFactory));

            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(ISystemProcess entity)
        {
            if (entity == null || string.IsNullOrWhiteSpace(entity.Id)) return;

            try
            {
                this._logger.LogInformation($"SystemProcessRepository SAVING {entity}");
                using (var dbConnection = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.ExecuteAsync(CreateSql, entity))
                {
                    await conn;
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"SystemProcessRepository Create Method For {entity?.Id} {entity.MachineId}");
            }
        }

        public async Task<IReadOnlyCollection<ISystemProcess>> ExpiredProcessesWithQueues()
        {
            try
            {
                var twoDaysAgo = DateTime.UtcNow.AddDays(-2);
                var cancelDate = $"{twoDaysAgo.Year}-{twoDaysAgo.Month}-{twoDaysAgo.Day}";

                using (var dbConnection = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.QueryAsync<SystemProcess>(
                    GetExpiredProcessesSql,
                    new { CancelParamDate = cancelDate }))
                {
                    var result = await conn;
                    return result.ToList();
                }
            }
            catch (Exception e)
            {
                this._logger.LogError($"SystemProcessRepository get dashboard method {e.Message}");
            }

            return new ISystemProcess[0];
        }

        public async Task<IReadOnlyCollection<ISystemProcess>> GetDashboard()
        {
            try
            {
                using (var dbConnection = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.QueryAsync<SystemProcess>(GetDashboardSql))
                {
                    var result = await conn;
                    return result.ToList();
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"SystemProcessRepository get dashboard method");
            }

            return new ISystemProcess[0];
        }

        public async Task Update(ISystemProcess entity)
        {
            if (entity == null || string.IsNullOrWhiteSpace(entity.Id)) return;

            try
            {
                using (var dbConnection = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.ExecuteAsync(UpdateSql, entity))
                {
                    await conn;
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"SystemProcessRepository Update Method For {entity?.Id}");
            }
        }
    }
}