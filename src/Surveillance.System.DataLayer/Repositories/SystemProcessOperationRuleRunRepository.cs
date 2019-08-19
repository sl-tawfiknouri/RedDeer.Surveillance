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

    public class SystemProcessOperationRuleRunRepository : ISystemProcessOperationRuleRunRepository
    {
        private const string CreateSql =
            "INSERT INTO SystemProcessOperationRuleRun(SystemProcessOperationId, RuleDescription, RuleVersion, ScheduleRuleStart, ScheduleRuleEnd, CorrelationId, RuleParameterId, RuleTypeId, IsBackTest, IsForceRun) VALUES(@SystemProcessOperationId, @RuleDescription, @RuleVersion, @ScheduleRuleStart, @ScheduleRuleEnd, @CorrelationId, @RuleParameterId, @RuleTypeId, @IsBackTest, @IsForceRun); SELECT LAST_INSERT_ID();";

        private const string GetDashboardSql =
            "SELECT * FROM SystemProcessOperationRuleRun ORDER BY Id DESC LIMIT 100;";

        private const string GetSql =
            "SELECT * FROM SystemProcessOperationRuleRun WHERE SystemProcessOperationId = @Id;";

        private const string UpdateSql =
            "UPDATE SystemProcessOperationRuleRun SET RuleDescription = @RuleDescription, RuleVersion = @RuleVersion, ScheduleRuleStart = @ScheduleRuleStart, ScheduleRuleEnd = @ScheduleRuleEnd, RuleParameterId = @RuleParameterId, RuleTypeId = @RuleTypeId, IsBackTest = @IsBackTest, IsForceRun = @IsForceRun;";

        private readonly IConnectionStringFactory _dbConnectionFactory;

        private readonly object _lock = new object();

        private readonly ILogger<ISystemProcessOperationRuleRunRepository> _logger;

        public SystemProcessOperationRuleRunRepository(
            IConnectionStringFactory connectionFactory,
            ILogger<ISystemProcessOperationRuleRunRepository> logger)
        {
            this._dbConnectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(ISystemProcessOperationRuleRun entity)
        {
            if (entity == null) return;

            lock (this._lock)
            {
                try
                {
                    this._logger.LogInformation($"SystemProcessOperationRuleRunRepository SAVING {entity}");
                    using (var dbConnection = this._dbConnectionFactory.BuildConn())
                    using (var conn = dbConnection.QuerySingleAsync<int>(CreateSql, entity))
                    {
                        entity.Id = conn.Result;
                    }
                }
                catch (Exception e)
                {
                    this._logger.LogError(
                        $"System Process Operation Rule Run Repository Create Method For {entity.Id} {entity.SystemProcessOperationId}. {e.Message}");
                }
            }
        }

        public async Task<IReadOnlyCollection<ISystemProcessOperationRuleRun>> Get(
            IReadOnlyCollection<string> systemProcessOperationIds)
        {
            if (systemProcessOperationIds == null || !systemProcessOperationIds.Any())
                return new ISystemProcessOperationRuleRun[0];

            try
            {
                using (var dbConnection = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.QueryAsync<SystemProcessOperationRuleRun>(
                    GetSql,
                    new { Id = systemProcessOperationIds }))
                {
                    var result = await conn;
                    return result.ToList();
                }
            }
            catch (Exception e)
            {
                this._logger.LogError($"System Process Operation Rule Run Repository Get method {e.Message}");
            }

            return new ISystemProcessOperationRuleRun[0];
        }

        public async Task<IReadOnlyCollection<ISystemProcessOperationRuleRun>> GetDashboard()
        {
            try
            {
                using (var dbConnection = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.QueryAsync<SystemProcessOperationRuleRun>(GetDashboardSql))
                {
                    var result = await conn;
                    return result.ToList();
                }
            }
            catch (Exception e)
            {
                this._logger.LogError($"System Process Operation Rule Run Repository Get Dashboard method {e.Message}");
            }

            return new ISystemProcessOperationRuleRun[0];
        }

        public async Task Update(ISystemProcessOperationRuleRun entity)
        {
            if (entity == null) return;

            try
            {
                using (var dbConnection = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.ExecuteAsync(UpdateSql, entity))
                {
                    await conn;
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(
                    $"System Process Operation Rule Run Repository Update Method For {entity.Id} {entity.SystemProcessOperationId}. {e.Message}");
            }
        }
    }
}