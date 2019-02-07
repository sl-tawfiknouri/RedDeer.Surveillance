using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Surveillance.Systems.DataLayer.Interfaces;
using Surveillance.Systems.DataLayer.Processes;
using Surveillance.Systems.DataLayer.Processes.Interfaces;
using Surveillance.Systems.DataLayer.Repositories.Interfaces;

namespace Surveillance.Systems.DataLayer.Repositories
{
    public class SystemProcessOperationRepository : ISystemProcessOperationRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<ISystemProcessOperationRepository> _logger;
        private const string CreateSql = "INSERT INTO SystemProcessOperation(SystemProcessId, OperationStart, OperationEnd, OperationState)  VALUES(@SystemProcessId, @OperationStart, @OperationEnd, @OperationState); SELECT LAST_INSERT_ID();";
        private const string UpdateSql = "UPDATE SystemProcessOperation SET OperationStart = @OperationStart, OperationEnd = @OperationEnd, OperationState = @OperationState WHERE Id = @Id;";
        private const string GetDashboardSql = @"SELECT * FROM SystemProcessOperation ORDER BY Id DESC LIMIT 15;";

        public SystemProcessOperationRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<ISystemProcessOperationRepository> logger)
        {
            _dbConnectionFactory =
                dbConnectionFactory
                ?? throw new ArgumentNullException(nameof(dbConnectionFactory));

            _logger =
                logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(ISystemProcessOperation entity)
        {
            if (entity == null
                || string.IsNullOrWhiteSpace(entity.SystemProcessId))
            {
                return;
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                _logger.LogInformation($"SystemProcessOperationRepository SAVING {entity}");
                using (var conn = dbConnection.QuerySingleAsync<int>(CreateSql, entity))
                {
                    entity.Id = await conn;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"System Process Operation Repository Create Method For {entity.Id} {entity.OperationEnd}. {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        public async Task Update(ISystemProcessOperation entity)
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
                _logger.LogError($"System Process Operation Repository Update Method For {entity.Id} {entity.OperationEnd}. {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        public async Task<IReadOnlyCollection<ISystemProcessOperation>> GetDashboard()
        {
            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                using (var conn = dbConnection.QueryAsync<SystemProcessOperation>(GetDashboardSql))
                {
                    var result = await conn;
                    return result.ToList();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"System Process Operation Repository Get Dashboard {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            return new ISystemProcessOperation[0];
        }
    }
}
