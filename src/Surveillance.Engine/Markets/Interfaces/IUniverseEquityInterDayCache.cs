using System;
using System.Collections.Generic;
using Domain.Core.Financial;
using Domain.Equity.TimeBars;
using Domain.Markets;
using Surveillance.Engine.Rules.Rules;

namespace Surveillance.Engine.Rules.Markets.Interfaces
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