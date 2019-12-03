namespace Surveillance.Auditing.DataLayer.Repositories.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Dapper;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.DataLayer.Interfaces;
    using Surveillance.Auditing.DataLayer.Repositories.Exceptions.Interfaces;

    public class ExceptionRepository : IExceptionRepository
    {
        private const string ExceptionDashboardSql =
            @"SELECT Id as Id, Exception as ExceptionMessage, InnerException As InnerExceptionMessage, StackTrace, SystemProcessId, SystemProcessOperationId, SystemProcessOperationRuleRunId, SystemProcessOperationDistributeRuleId, SystemProcessOperationFileUploadId AS SystemProcessOperationUploadFileRuleId, SystemProcessOperationThirdPartyDataRequestId AS SystemProcessOperationThirdPartyDataRequestId FROM Exceptions ORDER BY ID DESC Limit 30;";

        private const string ExceptionInsertSql =
            @"INSERT INTO Exceptions(Exception, InnerException, StackTrace, SystemProcessId, SystemProcessOperationId, SystemProcessOperationRuleRunId, SystemProcessOperationDistributeRuleId, SystemProcessOperationFileUploadId, SystemProcessOperationThirdPartyDataRequestId) VALUES(@ExceptionMessage, @InnerExceptionMessage, @StackTrace, @SystemProcessId, @SystemProcessOperationId, @SystemProcessOperationRuleRunId, @SystemProcessOperationDistributeRuleId, @SystemProcessOperationUploadFileRuleId, @SystemProcessOperationThirdPartyDataRequestId)";

        private readonly IConnectionStringFactory _dbConnectionFactory;

        private readonly ILogger<ExceptionRepository> _logger;

        public ExceptionRepository(IConnectionStringFactory dbConnectionFactory, ILogger<ExceptionRepository> logger)
        {
            this._dbConnectionFactory =
                dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyCollection<ExceptionDto>> GetForDashboard()
        {
            try
            {
                using (var dbConnection = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.QueryAsync<ExceptionDto>(ExceptionDashboardSql))
                {
                    var result = await conn;
                    return result.ToList();
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"Exception in the Exception Repository");
            }

            return new ExceptionDto[0];
        }

        public void Save(ExceptionDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ExceptionMessage)) return;

            try
            {
                this._logger.LogError($"ExceptionRepository SAVING {dto}");

                using (var dbConnection = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.ExecuteAsync(ExceptionInsertSql, dto))
                {
                    conn.Wait();
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"Exception in the Exception Repository");
            }
        }
    }
}