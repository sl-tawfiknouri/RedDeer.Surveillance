using System;
using System.Threading.Tasks;
using Dapper;
using Domain.Surveillance.Rules;
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
                OrganisationalFactorValue,
                ParameterTuning)
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
                @OrganisationalFactorValue,
                @ParameterTuning); 
            SELECT LAST_INSERT_ID();";

        private const string HasDuplicateSql = @"
            SET @RuleId = (SELECT RuleId FROM RuleBreach WHERE Id = @ruleBreachId LIMIT 1);
            SET @OrganisationalFactorType = (SELECT OrganisationalFactorType FROM RuleBreach WHERE Id = @ruleBreachId LIMIT 1);
            SET @OrganisationalFactorValue = (SELECT OrganisationalFactorValue FROM RuleBreach WHERE Id = @ruleBreachId LIMIT 1);

            SELECT COUNT(*) FROM (
	            SELECT COUNT(*) AS CNT FROM RuleBreach AS rb
	            RIGHT OUTER JOIN RuleBreachOrders AS rbo
	            ON rb.Id = rbo.RuleBreachId
	            WHERE rb.Id <> @ruleBreachId
	            AND rb.RuleId = @RuleId
	            AND rb.OrganisationalFactorType = @OrganisationalFactorType
	            AND rb.OrganisationalFactorValue = @OrganisationalFactorValue
	            AND rbo.OrderId = ANY (
		            SELECT rbo.OrderId FROM RuleBreach AS rb
		            RIGHT OUTER JOIN RuleBreachOrders AS rbo
		            ON rb.Id = rbo.RuleBreachId
		            WHERE rb.Id = @ruleBreachId)
		            GROUP BY rbo.RuleBreachId) AS InnerCounts
            WHERE InnerCounts.CNT = (SELECT COUNT(*) FROM RuleBreach AS rb RIGHT OUTER JOIN RuleBreachOrders AS rbo ON rb.Id = rbo.RuleBreachId WHERE rb.Id = @ruleBreachId);";

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
                _logger.LogWarning($"saving rule was passed a null message. Returning.");
                return null;
            }

            try
            {
                _logger.LogInformation($"saving rule breach to repository");
                var dto = new RuleBreachDto(message);

                using (var dbConnection = _dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.QueryFirstOrDefaultAsync<long?>(SaveRuleBreachSql, dto))
                {
                    var result = await conn;

                    _logger.LogInformation($"completed saving rule breach to repository");

                    return result;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"error for Create {e.Message} - {e?.InnerException?.Message}");
            }

            return null;
        }
        
        public async Task<RuleBreach> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning($"get rule breach was passed a null message. Returning.");
                return null;
            }

            try
            {
                _logger.LogInformation($"fetching rule breaches");

                using (var dbConnection = _dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.QuerySingleAsync<RuleBreachDto>(GetRuleBreachSql, new { Id = id}))
                {
                    var result = await conn;

                    _logger.LogInformation($"completed fetching rule breach");

                    var mappedResult = Project(result);

                    return mappedResult;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"error for Create {e.Message} - {e?.InnerException?.Message}");
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
                _logger.LogInformation($"checking duplicates");
                using (var dbConnection = _dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.ExecuteScalarAsync<long>(HasDuplicateSql, new { RuleBreachId = ruleId }))
                {
                    var result = await conn;

                    _logger.LogInformation($"completed checking duplicates");

                    return result != 0;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"error for Create {e.Message} - {e?.InnerException?.Message}");
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
                dto.OrganisationalFactorType,
                dto.OrganisationalFactorValue,
                dto.ParameterTuning,
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
                ParameterTuning = message?.ParameterTuning ?? false;

                OrganisationalFactorType = message?.OrganisationalFactor ?? 0;
                OrganisationalFactorValue = message?.OrganisationalFactorValue ?? string.Empty;
            }

            public int? Id { get; set; }
            public string RuleId { get; set; }
            public string CorrelationId { get; set; }
            public bool IsBackTest { get; set; }
            public bool ParameterTuning { get; set; }
            public DateTime CreatedOn { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Venue { get; set; }
            public DateTime StartOfPeriodUnderInvestigation { get; set; }
            public DateTime EndOfPeriodUnderInvestigation { get; set; }
            public string AssetCfi { get; set; }
            public string ReddeerEnrichmentId { get; set; }
            public string SystemOperationId { get; set; }
            public int OrganisationalFactorType { get; set; }
            public string OrganisationalFactorValue { get; set; }
        }
    }
}
