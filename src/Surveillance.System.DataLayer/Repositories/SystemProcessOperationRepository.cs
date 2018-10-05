using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Surveillance.System.DataLayer.Interfaces;
using Surveillance.System.DataLayer.Processes.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.DataLayer.Repositories
{
    public class SystemProcessOperationRepository : ISystemProcessOperationRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<ISystemProcessOperationRepository> _logger;
        private const string CreateSql = "INSERT INTO SystemProcessOperation(SystemProcessId, OperationStart, OperationEnd, OperationState)  VALUES(@SystemProcessId, @OperationStart, @OperationEnd, @OperationState); SELECT LAST_INSERT_ID();";
        private const string UpdateSql = "UPDATE SystemProcessOperation SET OperationStart = @OperationStart, OperationEnd = @OperationEnd, OperationState = @OperationState WHERE Id = @Id;";

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
            if (entity?.Id == null)
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
    }
}
