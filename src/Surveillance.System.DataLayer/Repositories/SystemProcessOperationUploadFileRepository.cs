using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Surveillance.System.DataLayer.Interfaces;
using Surveillance.System.DataLayer.Processes.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.DataLayer.Repositories
{
    public class SystemProcessOperationUploadFileRepository : ISystemProcessOperationUploadFileRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<SystemProcessOperationUploadFileRepository> _logger;

        private const string CreateSql = @"INSERT INTO SystemProcessOperationUploadFile(SystemProcessOperationId, FilePath, FileType) VALUES (@SystemProcessOperationId, @FilePath, @FileType); SELECT LAST_INSERT_ID();";

        public SystemProcessOperationUploadFileRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<SystemProcessOperationUploadFileRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(ISystemProcessOperationUploadFile entity)
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
                _logger.LogError($"System Process Operation Upload File Repository Create Method For {entity.Id} {entity.SystemProcessId}. {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

    }
}
