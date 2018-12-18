using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Equity.Frames;
using DomainV2.Financial;
using DomainV2.Financial.Interfaces;
using DomainV2.Markets;
using Surveillance.Markets.Interfaces;

namespace Surveillance.Markets
{
    /// <summary>
    /// Universe event market cache
    /// </summary>
    public class UniverseMarketCache : IUniverseMarketCache
    {
        private readonly TimeSpan _windowSize;
        private readonly IDictionary<string, ExchangeFrame> _latestExchangeFrameBook;
        private readonly ConcurrentDictionary<string, IMarketHistoryStack> _marketHistory;

        public UniverseMarketCache(TimeSpan windowSize)
        {
            _windowSize = windowSize;
            _latestExchangeFrameBook = new ConcurrentDictionary<string, ExchangeFrame>();
            _marketHistory = new ConcurrentDictionary<string, IMarketHistoryStack>();
        }

        public void Add(ExchangeFrame value)
        {
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
                history.Add(value, value.TimeStamp);
                _marketHistory.TryAdd(value.Exchange.MarketIdentifierCode, history);
            }
            else
            {
                _marketHistory.TryGetValue(value.Exchange.MarketIdentifierCode, out var history);

                history?.Add(value, value.TimeStamp);
                history?.ArchiveExpiredActiveItems(value.TimeStamp);
            }
        }

        public MarketDataResponse<SecurityTick> Get(MarketDataRequest request)
        {
            if (request == null
                || !request.IsValid())
            {
                return MarketDataResponse<SecurityTick>.MissingData();
            }

            if (!_latestExchangeFrameBook.ContainsKey(request.MarketIdentifierCode))
            {
                return MarketDataResponse<SecurityTick>.MissingData();
            }

            _latestExchangeFrameBook.TryGetValue(request.MarketIdentifierCode, out var exchangeFrame);

            if (exchangeFrame == null)
            {
                return MarketDataResponse<SecurityTick>.MissingData();
            }

            var security = exchangeFrame
                .Securities
                .FirstOrDefault(sec => Equals(sec.Security.Identifiers, request.Identifiers));

            if (security == null)
            {
                return MarketDataResponse<SecurityTick>.MissingData();
            }

            return new MarketDataResponse<SecurityTick>(security, false);
        }

        public MarketDataResponse<List<SecurityTick>> GetMarkets(MarketDataRequest request)
        {
            if (request == null
                || !request.IsValid())
            {
                return MarketDataResponse<List<SecurityTick>>.MissingData();
            }

            if (!_marketHistory.TryGetValue(request.MarketIdentifierCode, out var marketStack))
            {
                return MarketDataResponse<List<SecurityTick>>.MissingData();
            }

            var securityDataTicks = marketStack
                .ActiveMarketHistory()
                .Where(amh => amh != null)
                .Select(amh =>
                    amh.Securities?.FirstOrDefault(sec =>
                        Equals(sec.Security.Identifiers, request.Identifiers)))
                .Where(sec => sec != null)
                .ToList();

            return new MarketDataResponse<List<SecurityTick>>(securityDataTicks, false);
        }
    }
}
