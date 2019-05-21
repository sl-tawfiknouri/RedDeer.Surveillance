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

namespace Surveillance.Auditing.DataLayer.Repositories
{
    public class SystemProcessRepository : ISystemProcessRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger _logger;
        private const string CreateSql = "INSERT INTO SystemProcess(Id, InstanceInitiated, Heartbeat, MachineId, ProcessId, SystemProcessTypeId, CancelRuleQueueDeletedFlag) VALUES(@Id, @InstanceInitiated, @Heartbeat, @MachineId, @ProcessId, @SystemProcessType, @CancelRuleQueueDeletedFlag)";
        private const string UpdateSql = "UPDATE SystemProcess SET Heartbeat = @Heartbeat, CancelRuleQueueDeletedFlag = @CancelRuleQueueDeletedFlag WHERE Id = @Id";
        private const string GetDashboardSql = "SELECT Id, InstanceInitiated, Heartbeat, MachineId, ProcessId, SystemProcessTypeId as SystemProcessType, CancelRuleQueueDeletedFlag FROM SystemProcess ORDER BY Heartbeat DESC LIMIT 10;";
        private const string GetExpiredProcessesSql = @"SELECT Id, InstanceInitiated, Heartbeat, MachineId, ProcessId, SystemProcessTypeId as SystemProcessType, CancelRuleQueueDeletedFlag 
            FROM SystemProcess
            WHERE CancelRuleQueueDeletedFlag = 0
            AND Heartbeat < @CancelParamDate;";

        public SystemProcessRepository(
            IConnectionStringFactory connectionStringFactory,
            ILogger<ISystemProcessRepository> logger)
        {
            _dbConnectionFactory =
                connectionStringFactory
                ?? throw new ArgumentNullException(nameof(connectionStringFactory));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(ISystemProcess entity)
        {
            if (entity == null
                || string.IsNullOrWhiteSpace(entity.Id))
            {
                return;
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                _logger.LogInformation($"SystemProcessRepository SAVING {entity}");
                using (var conn = dbConnection.ExecuteAsync(CreateSql, entity))
                {
                    await conn;
                }
            }
            catch(Exception e)
            {
                _logger.LogError($"SystemProcessRepository Create Method For {entity?.Id} {entity.MachineId} {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        public async Task Update(ISystemProcess entity)
        {
            if (entity == null
                || string.IsNullOrWhiteSpace(entity.Id))
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
                _logger.LogError($"SystemProcessRepository Update Method For {entity?.Id} {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        public async Task<IReadOnlyCollection<ISystemProcess>> ExpiredProcessesWithQueues()
        {
            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                var twoDaysAgo = DateTime.UtcNow.AddDays(-2);
                var cancelDate = $"{twoDaysAgo.Year}-{twoDaysAgo.Month}-{twoDaysAgo.Day}";

                using (var conn = dbConnection.QueryAsync<SystemProcess>(GetExpiredProcessesSql, new { CancelParamDate = cancelDate }))
                {
                    var result = await conn;
                    return result.ToList();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"SystemProcessRepository get dashboard method {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            return new ISystemProcess[0];
        }

        public async Task<IReadOnlyCollection<ISystemProcess>> GetDashboard()
        {
            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                using (var conn = dbConnection.QueryAsync<SystemProcess>(GetDashboardSql))
                {
                   var result = await conn;
                   return result.ToList();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"SystemProcessRepository get dashboard method {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            return new ISystemProcess[0];
        }
    }
}
