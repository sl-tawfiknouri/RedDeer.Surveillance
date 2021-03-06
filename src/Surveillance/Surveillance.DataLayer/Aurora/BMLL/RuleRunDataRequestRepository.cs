﻿namespace Surveillance.DataLayer.Aurora.BMLL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Dapper;

    using Domain.Core.Financial.Assets;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Markets;

    using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
    using Surveillance.DataLayer.Aurora.Interfaces;

    public class RuleRunDataRequestRepository : IRuleRunDataRequestRepository
    {
        private const string CheckDataRequestSql = @"
            SELECT EXISTS(SELECT * FROM RuleDataRequest WHERE SystemProcessOperationRuleRunId = @ruleRunId);";

        private const string CreateDataRequestSql = @"
            INSERT IGNORE INTO RuleDataRequest(MarketIdentifierCode, SystemProcessOperationRuleRunId, FinancialInstrumentId, StartTime, EndTime, Completed, DataSource) VALUES(@MarketIdentifierCode, @SystemProcessOperationRuleRunId, @FinancialInstrumentId, @StartTime, @EndTime, @Completed, @DataSource);";

        private const string GetDataRequestSql = @"
            SELECT DISTINCT
                 rdr.Id as Id,
                 rdr.MarketIdentifierCode,
                 rdr.SystemProcessOperationRuleRunId,
                 rdr.FinancialInstrumentId,
                 rdr.StartTime,
                 rdr.EndTime,
                 rdr.Completed,
                 rdr.DataSource,
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
                 fi.Ric as InstrumentRic,
                 fi.UnderlyingSedol as InstrumentUnderlyingSedol,
                 fi.UnderlyingIsin as InstrumentUnderlyingIsin,
                 fi.UnderlyingFigi as InstrumentUnderlyingFigi,
                 fi.UnderlyingCusip as InstrumentUnderlyingCusip,
                 fi.UnderlyingLei as InstrumentUnderlyingLei,
                 fi.UnderlyingExchangeSymbol as InstrumentUnderlyingExchangeSymbol,
                 fi.UnderlyingBloombergTicker as InstrumentUnderlyingBloombergTicker,
                 fi.UnderlyingClientIdentifier as InstrumentUnderlyingClientIdentifier,
                 fi.UnderlyingRic as InstrumentUnderlyingRic,
                 fi.Cfi as InstrumentCfi
             FROM RuleDataRequest as rdr
             LEFT OUTER JOIN FinancialInstruments as fi
             on rdr.FinancialInstrumentId = fi.Id
             LEFT OUTER JOIN SystemProcessOperationRuleRun as sporr
             on sporr.Id = rdr.SystemProcessOperationRuleRunId
             WHERE sporr.SystemProcessOperationId = @SystemOperationId;";

        private const string UpdateDataRequestAndDuplicatesSqlToComplete = @"
                UPDATE RuleDataRequest SET Completed = 1 WHERE (FinancialInstrumentId = @FinancialInstrumentId AND MarketIdentifierCode = @MarketIdentifierCode AND StartTime = @StartTime AND EndTime = @EndTime) OR Id = @Id;";

        private readonly IConnectionStringFactory _dbConnectionFactory;

        private readonly ILogger<RuleRunDataRequestRepository> _logger;

        public RuleRunDataRequestRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<RuleRunDataRequestRepository> logger)
        {
            this._dbConnectionFactory =
                dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CreateDataRequest(MarketDataRequest request)
        {
            this._logger.LogInformation(
                $"repository recording request for {request.MarketIdentifierCode} from {request.UniverseEventTimeFrom} to {request.UniverseEventTimeTo} for {request.Identifiers}.");

            if (string.IsNullOrWhiteSpace(request.Identifiers.Id)
                || string.IsNullOrWhiteSpace(request.SystemProcessOperationRuleRunId))
            {
                this._logger.LogError(
                    $"CreateDataRequest had a null or empty id for identifiers for financial instrument. {request.Identifiers.Id} - {request.SystemProcessOperationRuleRunId}");
                return;
            }

            try
            {
                this._logger.LogTrace(
                    $"CreateDataRequest about to save request for {request.Identifiers} at {request.UniverseEventTimeFrom} to {request.UniverseEventTimeTo}");
                var dtoRequest = new MarketDataRequestDto(request);

                using (var dbConnection = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.ExecuteAsync(CreateDataRequestSql, dtoRequest))
                {
                    await conn;
                    this._logger.LogTrace(
                        $"CreateDataRequest has saved request for {request.Identifiers} at {request.UniverseEventTimeFrom} to {request.UniverseEventTimeTo}");
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"repository error for CreateDataRequest");
            }
        }

        public async Task<IReadOnlyCollection<MarketDataRequest>> DataRequestsForSystemOperation(
            string systemOperationId)
        {
            if (string.IsNullOrWhiteSpace(systemOperationId)) return new MarketDataRequest[0];

            try
            {
                this._logger.LogInformation($"fetching market data requests for operation {systemOperationId}");
                using (var dbConnection = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.QueryAsync<MarketDataRequestDto>(
                    GetDataRequestSql,
                    new { SystemOperationId = systemOperationId }))
                {
                    var result = (await conn).ToList();

                    if (!result.Any())
                    {
                        this._logger.LogWarning(
                            $"fetched market data requests for operation {systemOperationId} and found no results");
                        return new MarketDataRequest[0];
                    }

                    this._logger.LogInformation(
                        $"fetched market data requests for operation {systemOperationId} and found {result.Count}");

                    var mappedResults = result.Select(this.Map).ToList();

                    return mappedResults;
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"Bmll data request repository error for DataRequestsForRuleRun");
            }

            return new MarketDataRequest[0];
        }

        public async Task<bool> HasDataRequestForRuleRun(string ruleRunId)
        {
            if (string.IsNullOrWhiteSpace(ruleRunId))
            {
                this._logger.LogTrace($"checked for null or empty {ruleRunId}");
                return false;
            }

            try
            {
                this._logger.LogInformation($"checking if there's any market data requests from rule run {ruleRunId}");
                using (var dbConnection = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.QueryFirstAsync<bool>(CheckDataRequestSql, new { ruleRunId }))
                {
                    var result = await conn;

                    this._logger.LogTrace(
                        $"checked if there's any market data requests and it was {result} for {ruleRunId}");

                    return result;
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"repository error for CreateDataRequest");
            }

            return false;
        }

        public async Task UpdateToCompleteWithDuplicates(IReadOnlyCollection<MarketDataRequest> requests)
        {
            if (requests == null || !requests.Any())
            {
                this._logger.LogInformation("received null or empty market data requests");
                return;
            }

            try
            {
                var requestDtos = requests.Where(i => !string.IsNullOrWhiteSpace(i.Id))
                    .Select(req => new MarketDataRequestDto(req)).ToList();

                this._logger.LogInformation("updating market data requests to set them to complete");

                using (var dbConnection = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.ExecuteAsync(UpdateDataRequestAndDuplicatesSqlToComplete, requestDtos))
                {
                    await conn;

                    this._logger.LogInformation("updated market data requests to set them to complete");
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"repository error for DataRequestsForRuleRun");
            }
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
                    re.InstrumentRic,
                    re.InstrumentUnderlyingSedol,
                    re.InstrumentUnderlyingIsin,
                    re.InstrumentUnderlyingFigi,
                    re.InstrumentUnderlyingCusip,
                    re.InstrumentUnderlyingLei,
                    re.InstrumentUnderlyingExchangeSymbol,
                    re.InstrumentUnderlyingBloombergTicker,
                    re.InstrumentUnderlyingClientIdentifier,
                    re.InstrumentUnderlyingRic),
                re.StartTime,
                re.EndTime,
                re.SystemProcessOperationRuleRunId,
                re.Completed,
                (DataSource)re.DataSource);
        }

        public class MarketDataRequestDto
        {
            public MarketDataRequestDto()
            {
                // used by dapper
            }

            public MarketDataRequestDto(MarketDataRequest dto)
            {
                this.Id = dto.Id;
                this.SystemProcessOperationRuleRunId = dto.SystemProcessOperationRuleRunId;
                this.MarketIdentifierCode = dto.MarketIdentifierCode?.ToUpper();
                this.InstrumentCfi = dto.Cfi?.ToUpper();
                this.FinancialInstrumentId = dto.Identifiers.Id;
                this.StartTime = dto.UniverseEventTimeFrom;
                this.EndTime = dto.UniverseEventTimeTo;
                this.Completed = dto.IsCompleted;
                this.DataSource = (int)dto.DataSource;
            }

            public bool Completed { get; set; }

            public int DataSource { get; set; }

            public DateTime? EndTime { get; set; }

            public string FinancialInstrumentId { get; set; }

            public string Id { get; set; }

            public string InstrumentBloombergTicker { get; set; }

            public string InstrumentCfi { get; set; }

            public string InstrumentClientIdentifier { get; set; }

            public string InstrumentCusip { get; set; }

            public string InstrumentExchangeSymbol { get; set; }

            public string InstrumentFigi { get; set; }

            public string InstrumentId { get; set; }

            public string InstrumentIsin { get; set; }

            public string InstrumentLei { get; set; }

            public string InstrumentReddeerId { get; set; }

            public string InstrumentSedol { get; set; }

            public string InstrumentRic { get; set; }

            public string InstrumentUnderlyingBloombergTicker { get; set; }

            public string InstrumentUnderlyingClientIdentifier { get; set; }

            public string InstrumentUnderlyingCusip { get; set; }

            public string InstrumentUnderlyingExchangeSymbol { get; set; }

            public string InstrumentUnderlyingFigi { get; set; }

            public string InstrumentUnderlyingIsin { get; set; }

            public string InstrumentUnderlyingLei { get; set; }

            public string InstrumentUnderlyingSedol { get; set; }

            public string InstrumentUnderlyingRic { get; set; }

            public string MarketIdentifierCode { get; set; }

            public DateTime? StartTime { get; set; }

            public string SystemProcessOperationRuleRunId { get; set; }
        }
    }
}