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

    public class SystemProcessOperationUploadFileRepository : ISystemProcessOperationUploadFileRepository
    {
        private const string CreateSql =
            @"INSERT INTO SystemProcessOperationUploadFile(SystemProcessOperationId, FilePath, FileType) VALUES (@SystemProcessOperationId, @FilePath, @FileType); SELECT LAST_INSERT_ID();";

        private const string GetByDate = @"
            SELECT UploadFile.*, SysOp.OperationStart AS FileUploadTime
            FROM SystemProcessOperationUploadFile AS UploadFile
            LEFT OUTER JOIN SystemProcessOperation AS SysOp
            ON UploadFile.SystemProcessOperationId = SysOp.Id
            WHERE SysOp.OperationStart >= @StartDate
            AND SysOp.OperationStart < @EndDate;";

        private const string GetDashboardSql =
            @"SELECT * FROM SystemProcessOperationUploadFile ORDER BY Id desc LIMIT 10;";

        private readonly IConnectionStringFactory _dbConnectionFactory;

        private readonly object _lock = new object();

        private readonly ILogger<SystemProcessOperationUploadFileRepository> _logger;

        public SystemProcessOperationUploadFileRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<SystemProcessOperationUploadFileRepository> logger)
        {
            this._dbConnectionFactory =
                dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(ISystemProcessOperationUploadFile entity)
        {
            if (entity == null) return;

            lock (this._lock)
            {
                try
                {
                    this._logger.LogInformation($"SystemProcessOperationUploadFileRepository SAVING {entity}");
                    using (var dbConnection = this._dbConnectionFactory.BuildConn())
                    using (var conn = dbConnection.QuerySingleAsync<int>(CreateSql, entity))
                    {
                        entity.Id = conn.Result;
                    }
                }
                catch (Exception e)
                {
                    this._logger.LogError(e, $"System Process Operation Upload File Repository Create Method For {entity.Id} {entity.SystemProcessId}.");
                }
            }
        }

        public async Task<IReadOnlyCollection<ISystemProcessOperationUploadFile>> GetDashboard()
        {
            try
            {
                using (var dbConnection = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.QueryAsync<SystemProcessOperationUploadFile>(GetDashboardSql))
                {
                    var result = await conn;
                    return result.ToList();
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"System Process Operation Upload File Repository Get Dashboard method");
            }

            return new ISystemProcessOperationUploadFile[0];
        }

        public async Task<IReadOnlyCollection<ISystemProcessOperationUploadFile>> GetOnDate(DateTime date)
        {
            var startDate = date.Date;
            var endDate = date.Date.AddDays(1).AddSeconds(-1);

            try
            {
                using (var dbConnection = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.QueryAsync<SystemProcessOperationUploadFile>(
                    GetByDate,
                    new { StartDate = startDate, EndDate = endDate }))
                {
                    var result = await conn;
                    return result.ToList();
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"System Process Operation Upload File Repository Get On Date method");
            }

            return new ISystemProcessOperationUploadFile[0];
        }
    }
}