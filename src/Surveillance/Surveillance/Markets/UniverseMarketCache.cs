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
using Surveillance.Markets.Interfaces;

namespace Surveillance.Markets
{
    /// <summary>
    /// Universe event market cache
    /// Consider that it is a cache of recent market events
    /// Before attempting to query for out of range data
    /// </summary>
    public class UniverseMarketCache : IUniverseMarketCache
    {
        private readonly TimeSpan _windowSize;
        private readonly IDictionary<string, MarketTimeBarCollection> _latestExchangeFrameBook;
        private readonly ConcurrentDictionary<string, IMarketHistoryStack> _marketHistory;
        private readonly IRuleRunDataRequestRepository _dataRequestRepository;
        private readonly ILogger _logger;

        public UniverseMarketCache(TimeSpan windowSize, IRuleRunDataRequestRepository dataRequestRepository, ILogger logger)
        {
            _windowSize = windowSize;
            _dataRequestRepository = dataRequestRepository ?? throw new ArgumentNullException(nameof(dataRequestRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _latestExchangeFrameBook = new ConcurrentDictionary<string, MarketTimeBarCollection>();
            _marketHistory = new ConcurrentDictionary<string, IMarketHistoryStack>();
        }

        public void Add(MarketTimeBarCollection value)
        {
            if (value == null)
            {
                _logger.LogInformation($"UniverseMarketCache was asked to add null. returning");
                return;
            }

            _logger.LogInformation($"UniverseMarketCache adding {value.Epoch} - {value.Exchange?.MarketIdentifierCode}");

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
                var history = new MarketHistoryStack(_windowSize);
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

        public MarketDataResponse<FinancialInstrumentTimeBar> Get(MarketDataRequest request)
        {
            if (request == null
                || !request.IsValid())
            {
                _logger.LogError($"UniverseMarketCache received either a null or invalid request");
                return MarketDataResponse<FinancialInstrumentTimeBar>.MissingData();
            }

            _logger.LogInformation($"UniverseMarketCache fetching for market {request?.MarketIdentifierCode} from {request?.UniverseEventTimeFrom} to {request?.UniverseEventTimeTo} as part of rule run {request?.SystemProcessOperationRuleRunId}");

            if (!_latestExchangeFrameBook.ContainsKey(request.MarketIdentifierCode))
            {
                _dataRequestRepository.CreateDataRequest(request);
                _logger.LogInformation($"UniverseMarketCache was not able to find the MIC {request.MarketIdentifierCode} in the latest exchange frame book. Recording missing data.");
                return MarketDataResponse<FinancialInstrumentTimeBar>.MissingData();
            }

            _latestExchangeFrameBook.TryGetValue(request.MarketIdentifierCode, out var exchangeFrame);

            if (exchangeFrame == null)
            {
                _dataRequestRepository.CreateDataRequest(request);
                _logger.LogInformation($"UniverseMarketCache was not able to find the MIC {request.MarketIdentifierCode} in the latest exchange frame book. Recording missing data.");
                return MarketDataResponse<FinancialInstrumentTimeBar>.MissingData();
            }

            var security = exchangeFrame
                .Securities
                .FirstOrDefault(sec => Equals(sec.Security.Identifiers, request.Identifiers));

            if (security == null)
            {
                _dataRequestRepository.CreateDataRequest(request);
                _logger.LogInformation($"UniverseMarketCache was not able to find the security {request.Identifiers} for MIC {request.MarketIdentifierCode} in the latest exchange frame book. Recording missing data.");
                return MarketDataResponse<FinancialInstrumentTimeBar>.MissingData();
            }

            if (exchangeFrame.Epoch > request.UniverseEventTimeTo
                || exchangeFrame.Epoch < request.UniverseEventTimeFrom)
            {
                _dataRequestRepository.CreateDataRequest(request);

                _logger.LogInformation($"UniverseMarketCache was not able to find the security {request.Identifiers} for MIC {request.MarketIdentifierCode} in the latest exchange frame book within a suitable data range to {request.UniverseEventTimeTo} from {request.UniverseEventTimeFrom}. Recording missing data.");

                return MarketDataResponse<FinancialInstrumentTimeBar>.MissingData();
            }

            _logger.LogInformation($"UniverseMarketCache was able to find a match for {request.Identifiers} returning data.");
            return new MarketDataResponse<FinancialInstrumentTimeBar>(security, false);
        }

        /// <summary>
        /// Assumes that any data implies that the whole data set/range is covered
        /// </summary>
        public MarketDataResponse<List<FinancialInstrumentTimeBar>> GetMarkets(MarketDataRequest request)
        {
            if (request == null
                || !request.IsValid())
            {
                _logger.LogError($"UniverseMarketCache received either a null or invalid request");
                return MarketDataResponse<List<FinancialInstrumentTimeBar>>.MissingData();
            }

            if (!_marketHistory.TryGetValue(request.MarketIdentifierCode, out var marketStack))
            {
                _logger.LogInformation($"UniverseMarketCache GetMarkets was not able to find a market history entry for {request.MarketIdentifierCode}");
                _dataRequestRepository.CreateDataRequest(request);
                return MarketDataResponse<List<FinancialInstrumentTimeBar>>.MissingData();
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
                _logger.LogInformation($"UniverseMarketCache GetMarkets was not able to find market data for the security on {request.MarketIdentifierCode} with ids {request.Identifiers}");

                _dataRequestRepository.CreateDataRequest(request);
                return MarketDataResponse<List<FinancialInstrumentTimeBar>>.MissingData();
            }

            _logger.LogInformation($"UniverseMarketCache GetMarkets was able to find a market history entry for {request.MarketIdentifierCode} and id {request.Identifiers}");
            return new MarketDataResponse<List<FinancialInstrumentTimeBar>>(securityDataTicks, false);
        }
    }
}
