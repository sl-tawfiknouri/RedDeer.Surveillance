using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Equity.TimeBars;
using DomainV2.Financial;
using DomainV2.Financial.Interfaces;
using DomainV2.Markets;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.Rules;

namespace Surveillance.Engine.Rules.Markets
{
    /// <summary>
    /// Universe event market cache
    /// Consider that it is a cache of recent market events
    /// Before attempting to query for out of range data
    /// </summary>
    public class UniverseEquityInterDayCache : IUniverseEquityInterDayCache
    {
        private IDictionary<string, EquityInterDayTimeBarCollection> _latestExchangeFrameBook;
        private ConcurrentDictionary<string, IInterDayHistoryStack> _marketHistory;

        private readonly IRuleRunDataRequestRepository _dataRequestRepository;
        private readonly ILogger _logger;

        public UniverseEquityInterDayCache(IRuleRunDataRequestRepository dataRequestRepository, ILogger logger)
        {
            _dataRequestRepository = dataRequestRepository ?? throw new ArgumentNullException(nameof(dataRequestRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _latestExchangeFrameBook = new ConcurrentDictionary<string, EquityInterDayTimeBarCollection>();
            _marketHistory = new ConcurrentDictionary<string, IInterDayHistoryStack>();
        }

        public void Add(EquityInterDayTimeBarCollection value)
        {
            if (value == null)
            {
                _logger.LogInformation($"UniverseEquityInterDayCache was asked to add null. returning");
                return;
            }

            _logger.LogInformation($"UniverseEquityInterDayCache adding {value.Epoch} - {value.Exchange?.MarketIdentifierCode}");

            if (_latestExchangeFrameBook.ContainsKey(value.Exchange.MarketIdentifierCode))
            {
                _latestExchangeFrameBook.Remove(value.Exchange.MarketIdentifierCode);
                _latestExchangeFrameBook.Add(value.Exchange.MarketIdentifierCode, value);
            }
            else
            {
                _latestExchangeFrameBook.Add(value.Exchange.MarketIdentifierCode, value);
            }

            if (!_marketHistory.ContainsKey(value.Exchange.MarketIdentifierCode))
            {
                var history = new InterDayHistoryStack();
                history.Add(value, value.Epoch);
                _marketHistory.TryAdd(value.Exchange.MarketIdentifierCode, history);
            }
            else
            {
                _marketHistory.TryGetValue(value.Exchange.MarketIdentifierCode, out var history);

                history?.Add(value, value.Epoch);
                history?.ArchiveExpiredActiveItems(value.Epoch);
            }
        }

        public MarketDataResponse<EquityInstrumentInterDayTimeBar> Get(MarketDataRequest request)
        {
            if (request == null
                || !request.IsValid())
            {
                _logger.LogError($"UniverseEquityInterDayCache received either a null or invalid request");
                return MarketDataResponse<EquityInstrumentInterDayTimeBar>.MissingData();
            }

            _logger.LogInformation($"UniverseEquityInterDayCache fetching for market {request?.MarketIdentifierCode} from {request?.UniverseEventTimeFrom} to {request?.UniverseEventTimeTo} as part of rule run {request?.SystemProcessOperationRuleRunId}");

            if (!_latestExchangeFrameBook.ContainsKey(request.MarketIdentifierCode))
            {
                _dataRequestRepository.CreateDataRequest(request);
                _logger.LogInformation($"UniverseEquityInterDayCache was not able to find the MIC {request.MarketIdentifierCode} in the latest exchange frame book. Recording missing data.");
                return MarketDataResponse<EquityInstrumentInterDayTimeBar>.MissingData();
            }

            _latestExchangeFrameBook.TryGetValue(request.MarketIdentifierCode, out var exchangeFrame);

            if (exchangeFrame == null)
            {
                _dataRequestRepository.CreateDataRequest(request);
                _logger.LogInformation($"UniverseEquityInterDayCache was not able to find the MIC {request.MarketIdentifierCode} in the latest exchange frame book. Recording missing data.");
                return MarketDataResponse<EquityInstrumentInterDayTimeBar>.MissingData();
            }

            var security = exchangeFrame
                .Securities
                .FirstOrDefault(sec => Equals(sec.Security.Identifiers, request.Identifiers));

            if (security == null)
            {
                _dataRequestRepository.CreateDataRequest(request);
                _logger.LogInformation($"UniverseEquityInterDayCache was not able to find the security {request.Identifiers} for MIC {request.MarketIdentifierCode} in the latest exchange frame book. Recording missing data.");
                return MarketDataResponse<EquityInstrumentInterDayTimeBar>.MissingData();
            }

            if (exchangeFrame.Epoch.Date >= request.UniverseEventTimeTo.GetValueOrDefault()
                && exchangeFrame.Epoch.Date <= request.UniverseEventTimeFrom.GetValueOrDefault())
            {
                _dataRequestRepository.CreateDataRequest(request);

                _logger.LogInformation($"UniverseEquityInterDayCache was not able to find the security {request.Identifiers} for MIC {request.MarketIdentifierCode} in the latest exchange frame book within a suitable data range to {request.UniverseEventTimeTo} from {request.UniverseEventTimeFrom}. Recording missing data.");

                return MarketDataResponse<EquityInstrumentInterDayTimeBar>.MissingData();
            }

            _logger.LogInformation($"UniverseEquityInterDayCache was able to find a match for {request.Identifiers} returning data.");
            return new MarketDataResponse<EquityInstrumentInterDayTimeBar>(security, false, false);
        }

        public MarketDataResponse<List<EquityInstrumentInterDayTimeBar>> GetMarketsForRange(
            MarketDataRequest request,
            IReadOnlyCollection<DateRange> dates,
            RuleRunMode runMode)
        {
            dates = dates?.Where(dat => dat != null)?.ToList();

            if (dates == null
                || !dates.Any())
            {
                _logger.LogError($"UniverseEquityInterDayCache GetMarketsForRange received either a null or invalid request (dates)");

                return MarketDataResponse<List<EquityInstrumentInterDayTimeBar>>.MissingData();
            }

            if (request == null
                || !request.IsValid())
            {
                _logger.LogError($"UniverseEquityInterDayCache GetMarketsForRange received either a null or invalid request");
                return MarketDataResponse<List<EquityInstrumentInterDayTimeBar>>.MissingData();
            }

            var projectedRequests = dates
                .Select(i => 
                    new MarketDataRequest(
                        null, 
                        request.MarketIdentifierCode,
                        request.Cfi,
                        request.Identifiers,
                        i.Start,
                        i.End,
                        request.SystemProcessOperationRuleRunId,
                        request.IsCompleted))
                .ToList();

            var responseList = new List<MarketDataResponse<List<EquityInstrumentInterDayTimeBar>>>();
            foreach (var paramSet in projectedRequests)
            {
                responseList.Add(GetMarkets(paramSet));
            }

            if (!responseList.Any())
            {
                _logger.LogInformation($"UniverseEquityInterDayCache GetMarketsForRange had missing data for rule run id {request.SystemProcessOperationRuleRunId}");
                return MarketDataResponse<List<EquityInstrumentInterDayTimeBar>>.MissingData();
            }

            var isMissingData = responseList.Any(o => o.HadMissingData);

            if (runMode == RuleRunMode.ValidationRun
                && isMissingData)
            {
                _logger.LogInformation($"UniverseEquityInterDayCache GetMarketsForRange was running a validation run and had missing data for rule run id {request.SystemProcessOperationRuleRunId}");
                return MarketDataResponse<List<EquityInstrumentInterDayTimeBar>>.MissingData();
            }

            var responses = responseList.Where(i => i.Response != null).SelectMany(i => i.Response).ToList();

            if (isMissingData)
            {
                _logger.LogInformation($"UniverseEquityInterDayCache GetMarketsForRange was running and had missing data for rule run id {request.SystemProcessOperationRuleRunId} but is proceeding on a best effort basis");
            }

            return new MarketDataResponse<List<EquityInstrumentInterDayTimeBar>>(responses, isMissingData, true);
        }

        /// <summary>
        /// Assumes that any data implies that the whole data set/range is covered
        /// </summary>
        public MarketDataResponse<List<EquityInstrumentInterDayTimeBar>> GetMarkets(MarketDataRequest request)
        {
            if (request == null
                || !request.IsValid())
            {
                _logger.LogError($"UniverseEquityInterDayCache received either a null or invalid request");
                return MarketDataResponse<List<EquityInstrumentInterDayTimeBar>>.MissingData();
            }

            if (!_marketHistory.TryGetValue(request.MarketIdentifierCode, out var marketStack))
            {
                _logger.LogInformation($"UniverseEquityInterDayCache GetMarkets was not able to find a market history entry for {request.MarketIdentifierCode}");
                _dataRequestRepository.CreateDataRequest(request);
                return MarketDataResponse<List<EquityInstrumentInterDayTimeBar>>.MissingData();
            }

            var securityDataTicks = marketStack
                .ActiveMarketHistory()
                .Where(amh => amh != null)
                .Select(amh =>
                    amh.Securities?.FirstOrDefault(sec =>
                        Equals(sec.Security.Identifiers, request.Identifiers)))
                .Where(sec => sec != null)
                .Where(sec => sec.TimeStamp <= request.UniverseEventTimeTo)
                .Where(sec => sec.TimeStamp >= request.UniverseEventTimeFrom)
                .ToList();

            if (!securityDataTicks.Any())
            {
                _logger.LogInformation($"UniverseEquityInterDayCache GetMarkets was not able to find market data for the security on {request.MarketIdentifierCode} with ids {request.Identifiers}");

                _dataRequestRepository.CreateDataRequest(request);
                return MarketDataResponse<List<EquityInstrumentInterDayTimeBar>>.MissingData();
            }

            _logger.LogInformation($"UniverseEquityInterDayCache GetMarkets was able to find a market history entry for {request.MarketIdentifierCode} and id {request.Identifiers}");

            return new MarketDataResponse<List<EquityInstrumentInterDayTimeBar>>(securityDataTicks, false, false);
        }

        public object Clone()
        {
            var clone = this.MemberwiseClone() as UniverseEquityInterDayCache;
            clone.SetClone();

            return clone;
        }

        public void SetClone()
        {
            _latestExchangeFrameBook = new Dictionary<string, EquityInterDayTimeBarCollection>(_latestExchangeFrameBook);
            _marketHistory = new ConcurrentDictionary<string, IInterDayHistoryStack>(_marketHistory);
        }
    }
}
