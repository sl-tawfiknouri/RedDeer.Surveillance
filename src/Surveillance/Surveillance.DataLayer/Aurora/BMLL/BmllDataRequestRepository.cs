using System;
using System.Threading.Tasks;
using Dapper;
using DomainV2.Markets;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.DataLayer.Aurora.Interfaces;

namespace Surveillance.DataLayer.Aurora.BMLL
{
    public class BmllDataRequestRepository : IBmllDataRequestRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<BmllDataRequestRepository> _logger;

        private const string CreateDataRequestSql = @"
            INSERT INTO RuleDataRequest(MarketIdentifierCode, SystemProcessOperationRuleRunId, FinancialInstrumentId, StartTime, EndTime, Completed) VALUES(@MarketIdentifierCode, @SystemProcessOperationRuleRunId, @FinancialInstrumentId, @StartTime, @EndTime, 0);";

        private const string CheckDataRequestSql = @"
            SELECT EXISTS(SELECT * FROM RuleDataRequest WHERE SystemProcessOperationRuleRunId = @ruleRunId);";

        public BmllDataRequestRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<BmllDataRequestRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CreateDataRequest(MarketDataRequest request)
        {
            _logger.LogInformation($"BMLL data request repository recording request for {request.MarketIdentifierCode} from {request.UniverseEventTimeFrom} to {request.UniverseEventTimeTo} for {request.Identifiers}.");

            if (string.IsNullOrWhiteSpace(request.Identifiers.Id)
                || string.IsNullOrWhiteSpace(request.SystemProcessOperationRuleRunId))
            {
                _logger.LogError($"BmllDataRequestRepository.CreateDataRequest had a null or empty id for identifiers for financial instrument. {request.Identifiers.Id} - {request.SystemProcessOperationRuleRunId}");
                return;
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                var dtoRequest = new MarketDataRequestDto(request);
                using (var conn = dbConnection.ExecuteAsync(CreateDataRequestSql, dtoRequest))
                {
                    await conn;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Bmll data request repository error for CreateDataRequest {e.Message} - {e?.InnerException?.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        public async Task<bool> HasDataRequestForRuleRun(string ruleRunId)
        {
            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                _logger.LogInformation($"BmllDataRequestRepository checking if there's any market data requests from rule run {ruleRunId}");
                using (var conn = dbConnection.QueryFirstAsync<bool>(CheckDataRequestSql, new { ruleRunId }))
                {
                    var result = await conn;

                    _logger.LogInformation($"BmllDataRequestRepository checked if there's any market data requests and it was {result} for {ruleRunId}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Bmll data request repository error for CreateDataRequest {e.Message} - {e?.InnerException?.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            return false;
        }

        public class MarketDataRequestDto
        {
            public MarketDataRequestDto(MarketDataRequest dto)
            {
                SystemProcessOperationRuleRunId = dto.SystemProcessOperationRuleRunId;
                MarketIdentifierCode = dto.MarketIdentifierCode;
                FinancialInstrumentId = dto.Identifiers.Id;
                StartTime = dto.UniverseEventTimeFrom;
                EndTime = dto.UniverseEventTimeTo;
            }

            public string SystemProcessOperationRuleRunId { get; set; }
            public string MarketIdentifierCode { get; set; }
            public string FinancialInstrumentId { get; set; }
            public DateTime? StartTime { get; set; }
            public DateTime? EndTime { get; set; }
        }
    }
}
