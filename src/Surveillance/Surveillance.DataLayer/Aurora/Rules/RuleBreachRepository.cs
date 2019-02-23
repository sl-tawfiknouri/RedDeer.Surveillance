﻿using System;
using System.Threading.Tasks;
using Dapper;
using Domain.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.DataLayer.Aurora.Rules.Interfaces;

namespace Surveillance.DataLayer.Aurora.Rules
{
    public class RuleBreachRepository : IRuleBreachRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<RuleBreachRepository> _logger;

        private const string SaveRuleBreachSql =
            @"INSERT IGNORE INTO RuleBreach
                (RuleId, 
                CorrelationId, 
                IsBackTest, 
                CreatedOn, 
                Title, 
                Description, 
                Venue, 
                StartOfPeriodUnderInvestigation, 
                EndOfPeriodUnderInvestigation, 
                AssetCfi, 
                ReddeerEnrichmentId, 
                SystemOperationId,
                OrganisationalFactorType,
                OrganisationalFactorValue)
            VALUES(
                @RuleId,
                @CorrelationId,
                @IsBackTest, 
                @CreatedOn,
                @Title, 
                @Description, 
                @Venue, 
                @StartOfPeriodUnderInvestigation, 
                @EndOfPeriodUnderInvestigation, 
                @AssetCfi, 
                @ReddeerEnrichmentId, 
                @SystemOperationId,
                @OrganisationalFactorType,
                @OrganisationalFactorValue); 
            SELECT LAST_INSERT_ID();";

        private const string HasDuplicateSql = @"
            SELECT COUNT(*) FROM (
            SELECT rbo.OrderId 
	            FROM RuleBreach AS rb
	            LEFT OUTER JOIN RuleBreachOrders as rbo
	            ON rb.Id = rbo.RuleBreachId
	            WHERE rb.Id = @ruleBreachId
            AND NOT rbo.OrderId IN (SELECT rbo.OrderId 
            FROM RuleBreach AS rb
            LEFT OUTER JOIN RuleBreachOrders AS rbo
            ON rb.Id = rbo.RuleBreachId
            WHERE rb.RuleId = (SELECT RuleId FROM RuleBreach WHERE Id = @ruleBreachId)
            AND Id <> @ruleBreachId)) AS DuplicatesCheck;";

        private const string GetRuleBreachSql = @"SELECT * FROM RuleBreach WHERE Id = @Id";

        public RuleBreachRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<RuleBreachRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<long?> Create(RuleBreach message)
        {
            if (message == null)
            {
                _logger.LogWarning($"{nameof(RuleBreachRepository)} saving rule was passed a null message. Returning.");
                return null;
            }

            try
            {
                _logger.LogInformation($"{nameof(RuleBreachRepository)} saving rule breach to repository");
                var dto = new RuleBreachDto(message);

                using (var dbConnection = _dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.QueryFirstOrDefaultAsync<long?>(SaveRuleBreachSql, dto))
                {
                    var result = await conn;

                    _logger.LogInformation($"{nameof(RuleBreachRepository)} completed saving rule breach to repository");

                    return result;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(RuleBreachRepository)} error for Create {e.Message} - {e?.InnerException?.Message}");
            }

            return null;
        }
        
        public async Task<RuleBreach> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning($"{nameof(RuleBreachRepository)} get rule breach was passed a null message. Returning.");
                return null;
            }

            try
            {
                _logger.LogInformation($"{nameof(RuleBreachRepository)} fetching rule breaches");

                using (var dbConnection = _dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.QuerySingleAsync<RuleBreachDto>(GetRuleBreachSql, new { Id = id}))
                {
                    var result = await conn;

                    _logger.LogInformation($"{nameof(RuleBreachRepository)} completed fetching rule breach");

                    var mappedResult = Project(result);

                    return mappedResult;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(RuleBreachRepository)} error for Create {e.Message} - {e?.InnerException?.Message}");
            }

            return null;
        }

        public async Task<bool> HasDuplicate(string ruleId)
        {
            if (string.IsNullOrWhiteSpace(ruleId))
            {
                return false;
            }

            try
            {
                _logger.LogInformation($"{nameof(RuleBreachRepository)} checking duplicates");
                using (var dbConnection = _dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.ExecuteScalarAsync<long>(HasDuplicateSql, new { RuleBreachId = ruleId }))
                {
                    var result = await conn;

                    _logger.LogInformation($"{nameof(RuleBreachRepository)} completed checking duplicates");

                    return result == 0;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(RuleBreachRepository)} error for Create {e.Message} - {e?.InnerException?.Message}");
            }

            return false;
        }

        private RuleBreach Project(RuleBreachDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new RuleBreach(
                dto.Id,
                dto.RuleId,
                dto.CorrelationId,
                dto.IsBackTest,
                dto.CreatedOn,
                dto.Title,
                dto.Description,
                dto.Venue,
                dto.StartOfPeriodUnderInvestigation,
                dto.EndOfPeriodUnderInvestigation, 
                dto.AssetCfi, 
                dto.ReddeerEnrichmentId,
                dto.SystemOperationId,
                dto.OrganisationalFactor,
                dto.OrganisationalFactorValue,
                new int[0]);
        }

        /// <summary>
        /// Database dto for rule breaches
        /// </summary>
        public class RuleBreachDto
        {
            public RuleBreachDto()
            {
                // leave blank ctor  
            }

            public RuleBreachDto(RuleBreach message)
            {
                if (message == null)
                {
                    return;
                }

                RuleId = message.RuleId;
                CorrelationId = message.CorrelationId;
                IsBackTest = message.IsBackTest;
                CreatedOn = DateTime.UtcNow;
                Title = message?.Title;
                Description = message?.Description;
                Venue = message?.Venue;
                StartOfPeriodUnderInvestigation = message.StartOfPeriodUnderInvestigation;
                EndOfPeriodUnderInvestigation = message.EndOfPeriodUnderInvestigation;
                AssetCfi = message?.AssetCfi;
                SystemOperationId = message?.SystemOperationId;
                ReddeerEnrichmentId = message?.ReddeerEnrichmentId;

                OrganisationalFactor = message?.OrganisationalFactor ?? 0;
                OrganisationalFactorValue = message?.OrganisationalFactorValue ?? string.Empty;
            }

            public int? Id { get; set; }
            public string RuleId { get; set; }
            public string CorrelationId { get; set; }
            public bool IsBackTest { get; set; }
            public DateTime CreatedOn { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Venue { get; set; }
            public DateTime StartOfPeriodUnderInvestigation { get; set; }
            public DateTime EndOfPeriodUnderInvestigation { get; set; }
            public string AssetCfi { get; set; }
            public string ReddeerEnrichmentId { get; set; }
            public string SystemOperationId { get; set; }
            public int OrganisationalFactor { get; set; }
            public string OrganisationalFactorValue { get; set; }
        }
    }
}
