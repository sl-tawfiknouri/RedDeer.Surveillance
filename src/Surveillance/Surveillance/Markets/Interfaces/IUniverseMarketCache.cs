using System.Collections.Generic;
using DomainV2.Equity.TimeBars;
using DomainV2.Markets;

namespace Surveillance.Markets.Interfaces
{
    public interface IUniverseMarketCache
    {
        void Add(MarketTimeBarCollection value);
        MarketDataResponse<FinancialInstrumentTimeBar> Get(MarketDataRequest request);
        MarketDataResponse<List<FinancialInstrumentTimeBar>> GetMarkets(MarketDataRequest request);
    }
}