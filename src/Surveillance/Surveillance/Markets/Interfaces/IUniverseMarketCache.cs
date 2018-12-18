using System.Collections.Generic;
using DomainV2.Equity.Frames;
using DomainV2.Markets;

namespace Surveillance.Markets.Interfaces
{
    public interface IUniverseMarketCache
    {
        void Add(ExchangeFrame value);
        MarketDataResponse<SecurityTick> Get(MarketDataRequest request);
        MarketDataResponse<List<SecurityTick>> GetMarkets(MarketDataRequest request);
    }
}