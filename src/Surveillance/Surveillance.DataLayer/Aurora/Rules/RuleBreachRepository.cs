using System;
using System.Threading.Tasks;
using Dapper;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Interfaces;

namespace Surveillance.DataLayer.Aurora.Rules
{
    public class RuleBreachRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<RuleBreachRepository> _logger;

        private const string SaveRuleBreachSql = @"INSERT INTO RuleBreach (RuleId, CorrelationId, IsBackTest, CreatedOn, Title, Description, Venue, StartOfPeriodUnderInvestigation, EndOfPeriodUnderInvestigation, AssetCfi, ReddeerEnrichmentId, SystemOperationId) VALUES(@RuleId, @CorrelationId, @IsBackTest, @CreatedOn, @Title, @Description, @Venue, @StartOfPeriodUnderInvestigation, @EndOfPeriodUnderInvestigation, @AssetCfi, @ReddeerEnrichmentId, @SystemOperationId);";

        private const string GetRuleBreachSql = @"SELECT * FROM RuleBreach WHERE Id = @Id";

        public RuleBreachRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<RuleBreachRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(RuleBreach message)
        {
            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                _logger.LogInformation($"RuleBreachRepository saving rule breach to repository");
                var dto = new RuleBreachDto(message);
                using (var conn = dbConnection.ExecuteAsync(SaveRuleBreachSql, dto))
                {
                    await conn;

                    _logger.LogInformation($"RuleBreachRepository completed saving rule breach to repository");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"RuleBreachRepository error for Create {e.Message} - {e?.InnerException?.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        public async Task<RuleBreach> Get(string id)
        {
            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                _logger.LogInformation($"RuleBreachRepository fetching rule breaches");
                using (var conn = dbConnection.QuerySingleAsync<RuleBreachDto>(GetRuleBreachSql, new { Id = id}))
                {
                    var result = await conn;

                    _logger.LogInformation($"RuleBreachRepository completed fetching rule breach");

                    var mappedResult = Project(result);

                    return mappedResult;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"RuleBreachRepository error for Create {e.Message} - {e?.InnerException?.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            return null;
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
                dto.SystemOperationId);
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
            }

            public long Id { get; set; }
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
        }
    }
}
