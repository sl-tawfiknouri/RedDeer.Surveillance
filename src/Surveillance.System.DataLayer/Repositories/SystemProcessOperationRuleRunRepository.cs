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
    public class SystemProcessOperationRuleRunRepository : ISystemProcessOperationRuleRunRepository
    {
        private readonly object _lock = new object();
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<ISystemProcessOperationRuleRunRepository> _logger;

        private const string CreateSql = "INSERT INTO SystemProcessOperationRuleRun(SystemProcessOperationId, RuleDescription, RuleVersion, ScheduleRuleStart, ScheduleRuleEnd, CorrelationId, RuleParameterId, RuleTypeId, IsBackTest, IsForceRun) VALUES(@SystemProcessOperationId, @RuleDescription, @RuleVersion, @ScheduleRuleStart, @ScheduleRuleEnd, @CorrelationId, @RuleParameterId, @RuleTypeId, @IsBackTest, @IsForceRun); SELECT LAST_INSERT_ID();";
        private const string UpdateSql = "UPDATE SystemProcessOperationRuleRun SET RuleDescription = @RuleDescription, RuleVersion = @RuleVersion, ScheduleRuleStart = @ScheduleRuleStart, ScheduleRuleEnd = @ScheduleRuleEnd, RuleParameterId = @RuleParameterId, RuleTypeId = @RuleTypeId, IsBackTest = @IsBackTest, IsForceRun = @IsForceRun;";

        private const string GetSql = "SELECT * FROM SystemProcessOperationRuleRun WHERE SystemProcessOperationId = @Id;";


        private const string GetDashboardSql = "SELECT * FROM SystemProcessOperationRuleRun ORDER BY Id DESC LIMIT 100;";

        public SystemProcessOperationRuleRunRepository(
            IConnectionStringFactory connectionFactory,
            ILogger<ISystemProcessOperationRuleRunRepository> logger)
        {
            _dbConnectionFactory =
                connectionFactory
                ?? throw new ArgumentNullException(nameof(connectionFactory));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(ISystemProcessOperationRuleRun entity)
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

                    _logger.LogInformation($"SystemProcessOperationRuleRunRepository SAVING {entity}");
                    using (var conn = dbConnection.QuerySingleAsync<int>(CreateSql, entity))
                    {
                        var connTask = conn;
                        connTask.Wait();
                        entity.Id = connTask.Result;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"System Process Operation Rule Run Repository Create Method For {entity.Id} {entity.SystemProcessOperationId}. {e.Message}");
                }
                finally
                {
                    dbConnection.Close();
                    dbConnection.Dispose();
                }
            }
        }

        public async Task Update(ISystemProcessOperationRuleRun entity)
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
                _logger.LogError($"System Process Operation Rule Run Repository Update Method For {entity.Id} {entity.SystemProcessOperationId}. {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        public async Task<IReadOnlyCollection<ISystemProcessOperationRuleRun>> GetDashboard()
        {
            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                using (var conn = dbConnection.QueryAsync<SystemProcessOperationRuleRun>(GetDashboardSql))
                {
                    var result = await conn;
                    return result.ToList();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"System Process Operation Rule Run Repository Get Dashboard method {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            return new ISystemProcessOperationRuleRun[0];
        }

        public async Task<IReadOnlyCollection<ISystemProcessOperationRuleRun>> Get(IReadOnlyCollection<string> systemProcessOperationIds)
        {
            if (systemProcessOperationIds == null
                || !systemProcessOperationIds.Any())
            {
                return new ISystemProcessOperationRuleRun[0];
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                using (var conn = dbConnection.QueryAsync<SystemProcessOperationRuleRun>(GetSql, new {Id = systemProcessOperationIds}))
                {
                    var result = await conn;
                    return result.ToList();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"System Process Operation Rule Run Repository Get method {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            return new ISystemProcessOperationRuleRun[0];
        }
    }
}