using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DomainV2.Financial;
using DomainV2.Markets;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.DataLayer.Aurora.Interfaces;

namespace Surveillance.DataLayer.Aurora.BMLL
{
    public class RuleRunDataRequestRepository : IRuleRunDataRequestRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<RuleRunDataRequestRepository> _logger;

        private const string CreateDataRequestSql = @"
            INSERT INTO RuleDataRequest(MarketIdentifierCode, SystemProcessOperationRuleRunId, FinancialInstrumentId, StartTime, EndTime, Completed) VALUES(@MarketIdentifierCode, @SystemProcessOperationRuleRunId, @FinancialInstrumentId, @StartTime, @EndTime, @Completed);";

        private const string CheckDataRequestSql = @"
            SELECT EXISTS(SELECT * FROM RuleDataRequest WHERE SystemProcessOperationRuleRunId = @ruleRunId);";

        private const string GetDataRequestSql = @"
            SELECT DISTINCT
                 rdr.Id as Id,
                 rdr.MarketIdentifierCode,
                 rdr.SystemProcessOperationRuleRunId,
                 rdr.FinancialInstrumentId,
                 rdr.StartTime,
                 rdr.EndTime,
                 rdr.Completed,
                 fi.Id as InstrumentId,
                 fi.ReddeerId as InstrumentReddeerId,
                 fi.ClientIdentifier as InstrumentClientIdentifier,
                 fi.Sedol as InstrumentSedol,
                 fi.Isin as InstrumentIsin,
                 fi.Figi as InstrumentFigi,
                 fi.Cusip as InstrumentCusip,
                 fi.ExchangeSymbol as InstrumentExchangeSymbol,
                 fi.Lei as InstrumentLei,
                 fi.BloombergTicker as InstrumentBloombergTicker,
                 fi.UnderlyingSedol as InstrumentUnderlyingSedol,
                 fi.UnderlyingIsin as InstrumentUnderlyingIsin,
                 fi.UnderlyingFigi as InstrumentUnderlyingFigi,
                 fi.UnderlyingCusip as InstrumentUnderlyingCusip,
                 fi.UnderlyingLei as InstrumentUnderlyingLei,
                 fi.UnderlyingExchangeSymbol as InstrumentUnderlyingExchangeSymbol,
                 fi.UnderlyingBloombergTicker as InstrumentUnderlyingBloombergTicker,
                 fi.UnderlyingClientIdentifier as InstrumentUnderlyingClientIdentifier,
                 fi.Cfi as InstrumentCfi
             FROM RuleDataRequest as rdr
             LEFT OUTER JOIN FinancialInstruments as fi
             on rdr.FinancialInstrumentId = fi.Id
             WHERE rdr.SystemProcessOperationRuleRunId = @ruleRunId;";

        private const string UpdateDataRequestAndDuplicatesSqlToComplete = @"
                UPDATE RuleDataRequest SET Completed = 1 WHERE (FinancialInstrumentId = @FinancialInstrumentId AND MarketIdentifierCode = @MarketIdentifierCode AND StartTime = @StartTime AND EndTime = @EndTime) OR Id = @Id;";

        public RuleRunDataRequestRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<RuleRunDataRequestRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyCollection<MarketDataRequest>> DataRequestsForRuleRun(string ruleRunId)
        {
            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                _logger.LogInformation($"BmllDataRequestRepository fetching market data requests for rule run {ruleRunId}");
                using (var conn = dbConnection.QueryAsync<MarketDataRequestDto>(GetDataRequestSql, new { ruleRunId }))
                {
                    var result = (await conn).ToList();

                    if (!result.Any())
                    {
                        _logger.LogWarning($"BmllDataRequestRepository fetched market data requests for rule run {ruleRunId} and found no results");
                        return new MarketDataRequest[0];
                    }

                    _logger.LogInformation($"BmllDataRequestRepository fetched market data requests for rule run {ruleRunId} and found {result.Count}");

                    var mappedResults =
                        result
                            .Select(Map)
                            .ToList();

                    return mappedResults;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Bmll data request repository error for DataRequestsForRuleRun {e.Message} - {e?.InnerException?.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            return new MarketDataRequest[0];
        }

        public async Task UpdateToCompleteWithDuplicates(IReadOnlyCollection<MarketDataRequest> requests)
        {
            if (requests == null
                || !requests.Any())
            {
                _logger.LogInformation($"BmllDataRequestRepository received null or empty market data requests");
                return;
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                var requestDtos = requests.Where(i => !string.IsNullOrWhiteSpace(i.Id)).Select(req => new MarketDataRequestDto(req)).ToList();
                _logger.LogInformation($"BmllDataRequestRepository updating market data requests to set them to complete");
                using (var conn = dbConnection.ExecuteAsync(UpdateDataRequestAndDuplicatesSqlToComplete, requestDtos))
                {
                     await conn;

                    _logger.LogInformation($"BmllDataRequestRepository updated market data requests to set them to complete");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Bmll data request repository error for DataRequestsForRuleRun {e.Message} - {e?.InnerException?.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
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

                _logger.LogInformation($"BmllDataRequestRepository CreateDataRequest about to save bmll request for {request.Identifiers} at {request.UniverseEventTimeFrom} to {request.UniverseEventTimeTo}");
                var dtoRequest = new MarketDataRequestDto(request);
                using (var conn = dbConnection.ExecuteAsync(CreateDataRequestSql, dtoRequest))
                {
                    await conn;
                    _logger.LogInformation($"BmllDataRequestRepository CreateDataRequest has saved bmll request for {request.Identifiers} at {request.UniverseEventTimeFrom} to {request.UniverseEventTimeTo}");
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

        private MarketDataRequest Map(MarketDataRequestDto re)
        {
            return new MarketDataRequest(
                re.Id,
                re.MarketIdentifierCode,
                re.InstrumentCfi,
                new InstrumentIdentifiers(
                    re.InstrumentId,
                    re.InstrumentReddeerId,
                    string.Empty,
                    re.InstrumentClientIdentifier,
                    re.InstrumentSedol,
                    re.InstrumentIsin,
                    re.InstrumentFigi,
                    re.InstrumentCusip,
                    re.InstrumentExchangeSymbol,
                    re.InstrumentLei,
                    re.InstrumentBloombergTicker,
                    re.InstrumentUnderlyingSedol,
                    re.InstrumentUnderlyingIsin,
                    re.InstrumentUnderlyingFigi,
                    re.InstrumentUnderlyingCusip,
                    re.InstrumentUnderlyingLei,
                    re.InstrumentUnderlyingExchangeSymbol,
                    re.InstrumentUnderlyingBloombergTicker,
                    re.InstrumentUnderlyingClientIdentifier),
                re.StartTime,
                re.EndTime,
                re.SystemProcessOperationRuleRunId,
                re.Completed);
        }

        public class MarketDataRequestDto
        {
            public MarketDataRequestDto()
            {
                // used by dapper
            }

            public MarketDataRequestDto(MarketDataRequest dto)
            {
                Id = dto.Id;
                SystemProcessOperationRuleRunId = dto.SystemProcessOperationRuleRunId;
                MarketIdentifierCode = dto.MarketIdentifierCode?.ToUpper();
                InstrumentCfi = dto.Cfi?.ToUpper();
                FinancialInstrumentId = dto.Identifiers.Id;
                StartTime = dto.UniverseEventTimeFrom;
                EndTime = dto.UniverseEventTimeTo;
                Completed = dto.IsCompleted;
            }

            public string Id { get; set; }

            public string SystemProcessOperationRuleRunId { get; set; }
            public string MarketIdentifierCode { get; set; }
            public string FinancialInstrumentId { get; set; }
            public DateTime? StartTime { get; set; }
            public DateTime? EndTime { get; set; }
            public string InstrumentCfi { get; set; }
            public bool Completed { get; set; }

            public string InstrumentId { get; set; }
            public string InstrumentReddeerId { get; set; }
            public string InstrumentClientIdentifier { get; set; }
            public string InstrumentSedol { get; set; }
            public string InstrumentIsin { get; set; }
            public string InstrumentFigi { get; set; }
            public string InstrumentCusip { get; set; }
            public string InstrumentExchangeSymbol { get; set; }
            public string InstrumentLei { get; set; }
            public string InstrumentBloombergTicker { get; set; }
            public string InstrumentUnderlyingSedol { get; set; }
            public string InstrumentUnderlyingIsin { get; set; }
            public string InstrumentUnderlyingFigi { get; set; }
            public string InstrumentUnderlyingCusip { get; set; }
            public string InstrumentUnderlyingLei { get; set; }
            public string InstrumentUnderlyingExchangeSymbol { get; set; }
            public string InstrumentUnderlyingBloombergTicker { get; set; }
            public string InstrumentUnderlyingClientIdentifier { get; set; }
        }
    }
}
