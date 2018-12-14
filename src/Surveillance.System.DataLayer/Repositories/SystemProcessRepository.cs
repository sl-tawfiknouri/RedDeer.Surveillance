using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Surveillance.System.DataLayer.Interfaces;
using Surveillance.System.DataLayer.Processes;
using Surveillance.System.DataLayer.Processes.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.DataLayer.Repositories
{
    public class SystemProcessRepository : ISystemProcessRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger _logger;
        private const string CreateSql = "INSERT INTO SystemProcess(Id, InstanceInitiated, Heartbeat, MachineId, ProcessId, SystemProcessTypeId) VALUES(@Id, @InstanceInitiated, @Heartbeat, @MachineId, @ProcessId, @SystemProcessType)";
        private const string UpdateSql = "UPDATE SystemProcess SET Heartbeat = @Heartbeat WHERE Id = @Id";
        private const string GetDashboardSql = "SELECT Id, InstanceInitiated, Heartbeat, MachineId, ProcessId, SystemProcessTypeId as SystemProcessType FROM SystemProcess ORDER BY Heartbeat DESC LIMIT 10;";

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
