using System;
using System.Collections.Generic;
using DomainV2.Equity.TimeBars;
using DomainV2.Financial;
using DomainV2.Markets;
using Surveillance.Rules;

namespace Surveillance.Markets.Interfaces
{
    public interface IUniverseEquityInterDayCache : ICloneable
    {
        void Add(EquityInterDayTimeBarCollection value);
        MarketDataResponse<EquityInstrumentInterDayTimeBar> Get(MarketDataRequest request);
        MarketDataResponse<List<EquityInstrumentInterDayTimeBar>> GetMarkets(MarketDataRequest request);
        MarketDataResponse<List<EquityInstrumentInterDayTimeBar>> GetMarketsForRange(
            MarketDataRequest request,
            IReadOnlyCollection<DateRange> dates,
            RuleRunMode runMode);
    }
}