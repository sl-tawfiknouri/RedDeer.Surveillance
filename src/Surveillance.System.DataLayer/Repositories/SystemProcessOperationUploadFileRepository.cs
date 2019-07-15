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
    public class SystemProcessOperationUploadFileRepository : ISystemProcessOperationUploadFileRepository
    {
        private readonly object _lock = new object();
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<SystemProcessOperationUploadFileRepository> _logger;

        private const string CreateSql = @"INSERT INTO SystemProcessOperationUploadFile(SystemProcessOperationId, FilePath, FileType) VALUES (@SystemProcessOperationId, @FilePath, @FileType); SELECT LAST_INSERT_ID();";

        private const string GetDashboardSql = @"SELECT * FROM SystemProcessOperationUploadFile ORDER BY Id desc LIMIT 10;";

        private const string GetByDate = @"
            SELECT UploadFile.*, SysOp.OperationStart AS FileUploadTime
            FROM SystemProcessOperationUploadFile AS UploadFile
            LEFT OUTER JOIN SystemProcessOperation AS SysOp
            ON UploadFile.SystemProcessOperationId = SysOp.Id
            WHERE SysOp.OperationStart >= @StartDate
            AND SysOp.OperationStart < @EndDate";

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

            lock (_lock)
            {
                var dbConnection = _dbConnectionFactory.BuildConn();

                try
                {
                    dbConnection.Open();

                    _logger.LogInformation($"SystemProcessOperationUploadFileRepository SAVING {entity}");
                    using (var conn = dbConnection.QuerySingleAsync<int>(CreateSql, entity))
                    {
                        var connTask = conn;
                        connTask.Wait();
                        entity.Id = connTask.Result;
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

        public async Task<IReadOnlyCollection<ISystemProcessOperationUploadFile>> GetDashboard()
        {
            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                using (var conn = dbConnection.QueryAsync<SystemProcessOperationUploadFile>(GetDashboardSql))
                {
                    var result = await conn;
                    return result.ToList();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"System Process Operation Upload File Repository Get Dashboard method {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            return new ISystemProcessOperationUploadFile[0];
        }

        public async Task<IReadOnlyCollection<ISystemProcessOperationUploadFile>> GetOnDate(DateTime date)
        {
            var startDate = date.Date;
            var endDate = date.Date.AddDays(1).AddSeconds(-1);

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();
                
                using (var conn = dbConnection.QueryAsync<SystemProcessOperationUploadFile>(GetByDate, new { StartDate = startDate, EndDate = endDate }))
                {
                    var result = await conn;
                    return result.ToList();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"System Process Operation Upload File Repository Get On Date method {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            return new ISystemProcessOperationUploadFile[0];
        }
    }
}
