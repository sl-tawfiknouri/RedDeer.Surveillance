﻿using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Surveillance.System.DataLayer.Interfaces;
using Surveillance.System.DataLayer.Processes.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.DataLayer.Repositories
{
    public class SystemProcessOperationRuleRunRepository : ISystemProcessOperationRuleRunRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<ISystemProcessOperationRuleRunRepository> _logger;

        private const string CreateSql = "INSERT INTO SystemProcessOperationRuleRun(SystemProcessOperationId, RuleDescription, RuleVersion, ScheduleRuleStart, ScheduleRuleEnd, Alerts) VALUES(@SystemProcessOperationId, @RuleDescription, @RuleVersion, @ScheduleRuleStart, @ScheduleRuleEnd, @Alerts); SELECT LAST_INSERT_ID();";
        private const string UpdateSql = "UPDATE SystemProcessOperationRuleRun SET RuleDescription = @RuleDescription, RuleVersion = @RuleVersion, ScheduleRuleStart = @ScheduleRuleStart, ScheduleRuleEnd = @ScheduleRuleEnd, Alerts = @Alerts WHERE Id = @Id";

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
                _logger.LogError($"System Process Operation Repository Create Method For {entity.Id} {entity.SystemProcessOperationId}. {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
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
                _logger.LogError($"System Process Operation Repository Update Method For {entity.Id} {entity.SystemProcessOperationId}. {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }
    }
}