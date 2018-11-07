using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Surveillance.System.DataLayer.Interfaces;
using Surveillance.System.DataLayer.Repositories.Exceptions.Interfaces;

namespace Surveillance.System.DataLayer.Repositories.Exceptions
{
    public class ExceptionRepository : IExceptionRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<ExceptionRepository> _logger;

        private const string ExceptionInsertSql = @"INSERT INTO Exceptions(Exception, InnerException, StackTrace, SystemProcessId, SystemProcessOperationId, SystemProcessOperationRuleRunId, SystemProcessOperationDistributeRuleId, SystemProcessOperationFileUploadId) VALUES(@ExceptionMessage, @InnerExceptionMessage, @StackTrace, @SystemProcessId, @SystemProcessOperationId, @SystemProcessOperationRuleRunId, @SystemProcessOperationDistributeRuleId, @SystemProcessOperationFileUploadId)";

        private const string ExceptionDashboardSql = @"SELECT Id as Id, Exception as ExceptionMessage, InnerException As InnerExceptionMessage, StackTrace, SystemProcessId, SystemProcessOperationId, SystemProcessOperationRuleRunId, SystemProcessOperationDistributeRuleId, SystemProcessOperationFileUploadId FROM Exceptions ORDER BY ID DESC Limit 30;";

        public ExceptionRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<ExceptionRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Save(ExceptionDto dto)
        {
            if (dto == null
                || string.IsNullOrWhiteSpace(dto.ExceptionMessage))
            {
                return;
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                using (var conn = dbConnection.ExecuteAsync(ExceptionInsertSql, dto))
                {
                    conn.Wait();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception in the Exception Repository {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        public async Task<IReadOnlyCollection<ExceptionDto>> GetForDashboard()
        {
            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                using (var conn = dbConnection.QueryAsync<ExceptionDto>(ExceptionDashboardSql))
                {
                    var result = await conn;
                    return result.ToList();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception in the Exception Repository {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            return new ExceptionDto[0];
        }
    }
}
