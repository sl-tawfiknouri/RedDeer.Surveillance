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
    public class SystemProcessOperationRepository : ISystemProcessOperationRepository
    {
        private readonly object _lock = new object();
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

            lock (_lock)
            {
                try
                {
                    _logger.LogInformation($"SystemProcessOperationRepository SAVING {entity}");
                    using (var dbConnection = _dbConnectionFactory.BuildConn())
                    using (var conn = dbConnection.QuerySingleAsync<int>(CreateSql, entity))
                    {
                        entity.Id = conn.Result;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"System Process Operation Repository Create Method For {entity.Id} {entity.OperationEnd}. {e.Message}");
                }
            }
        }

        public async Task Update(ISystemProcessOperation entity)
        {
            if (entity == null)
            {
                return;
            }

            try
            {
                using (var dbConnection = _dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.ExecuteAsync(UpdateSql, entity))
                {
                    await conn;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"System Process Operation Repository Update Method For {entity.Id} {entity.OperationEnd}. {e.Message}");
            }
        }

        public async Task<IReadOnlyCollection<ISystemProcessOperation>> GetDashboard()
        {
            try
            {
                using (var dbConnection = _dbConnectionFactory.BuildConn())
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

            return new ISystemProcessOperation[0];
        }
    }
}
