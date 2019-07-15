﻿using System;
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
    public class SystemProcessOperationDistributeRuleRepository : ISystemProcessOperationDistributeRuleRepository
    {
        private readonly object _lock = new object();
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<ISystemProcessOperationDistributeRuleRepository> _logger;

        private const string CreateSql = "INSERT INTO SystemProcessOperationDistributeRule(SystemProcessOperationId, ScheduleRuleInitialStart, ScheduleRuleInitialEnd, RulesDistributed) VALUES(@SystemProcessOperationId, @ScheduleRuleInitialStart, @ScheduleRuleInitialEnd, @RulesDistributed); SELECT LAST_INSERT_ID();";

        private const string UpdateSql = "UPDATE SystemProcessOperationDistributeRule SET ScheduleRuleInitialStart = @ScheduleRuleInitialStart, ScheduleRuleInitialEnd = @ScheduleRuleInitialEnd, RulesDistributed = @RulesDistributed WHERE Id = @Id";

        private const string GetDashboardSql = "SELECT * FROM SystemProcessOperationDistributeRule ORDER BY Id DESC LIMIT 10;";

        public SystemProcessOperationDistributeRuleRepository(
            IConnectionStringFactory connectionFactory,
            ILogger<ISystemProcessOperationDistributeRuleRepository> logger)
        {
            _dbConnectionFactory =
                connectionFactory
                ?? throw new ArgumentNullException(nameof(connectionFactory));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task Create(ISystemProcessOperationDistributeRule entity)
        {
            if (entity == null)
            {
                return;
            }

            lock (_lock)
            {
                try
                {
                    _logger.LogInformation($"SystemProcessOperationDistributeRuleRepository SAVING {entity}");
                    using (var dbConnection = _dbConnectionFactory.BuildConn())
                    using (var conn = dbConnection.QuerySingleAsync<int>(CreateSql, entity))
                    {
                        entity.Id = conn.Result;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"System Process Operation Distribute Rule Repository Create Method For {entity.Id} {entity.SystemProcessOperationId}. {e.Message}");
                }
            }
        }

        public async Task Update(ISystemProcessOperationDistributeRule entity)
        {
            if (entity == null)
            {
                return;
            }
            
            try
            {
                using (var dbConnection = _dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.ExecuteAsync(UpdateSql, entity))
                {
                    await conn;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"System Process Operation Distribute Rule Repository Update Method For {entity.Id} {entity.SystemProcessOperationId}. {e.Message}");
            }
        }

        public async Task<IReadOnlyCollection<ISystemProcessOperationDistributeRule>> GetDashboard()
        {
            try
            {
                using (var dbConnection = _dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.QueryAsync<SystemProcessOperationDistributeRule>(GetDashboardSql))
                {
                    var result = await conn;
                    return result.ToList();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"System Process Operation Distribute Rule Repository Get Dashboard Method {e.Message}");
            }

            return new ISystemProcessOperationDistributeRule[0];
        }
    }
}
