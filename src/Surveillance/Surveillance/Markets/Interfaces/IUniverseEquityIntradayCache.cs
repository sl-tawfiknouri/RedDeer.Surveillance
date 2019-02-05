using System;
using System.Collections.Generic;
using DomainV2.Equity.TimeBars;
using DomainV2.Financial;
using DomainV2.Markets;
using Surveillance.Rules;

namespace Surveillance.Markets.Interfaces
{
    public interface IUniverseEquityIntradayCache : ICloneable
    {
        void Add(EquityIntraDayTimeBarCollection value);
        MarketDataResponse<EquityInstrumentIntraDayTimeBar> GetForLatestDayOnly(MarketDataRequest request);
        MarketDataResponse<List<EquityInstrumentIntraDayTimeBar>> GetMarkets(MarketDataRequest request);
        MarketDataResponse<List<EquityInstrumentIntraDayTimeBar>> GetMarketsForRange(
            MarketDataRequest request,
            IReadOnlyCollection<DateRange> dates,
            RuleRunMode runMode);
    }
}