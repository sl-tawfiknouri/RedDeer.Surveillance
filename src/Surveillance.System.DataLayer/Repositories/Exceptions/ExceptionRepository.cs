using System;
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

        private const string ExceptionInsertSql = @"INSERT INTO Exceptions(Exception, InnerException, StackTrace, SystemProcessId, SystemProcessOperationId, SystemProcessOperationruleRunId, SystemProcessOperationDistributeRuleId) VALUES(@ExceptionMessage, @InnerExceptionMessage, @StackTrace, @SystemProcessId, @SystemProcessOperationId, @SystemProcessOperationRuleRunId, @SystemProcessOperationDistributeRuleId)";

        public ExceptionRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<ExceptionRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Save(ExceptionDto dto)
        {
            if (dto == null)
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
    }
}
