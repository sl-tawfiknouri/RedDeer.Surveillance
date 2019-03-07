using System;
using System.Collections.Generic;
using Domain.Core.Dates;
using Domain.Core.Markets.Collections;
using Domain.Core.Markets.Timebars;
using SharedKernel.Contracts.Markets;
using Surveillance.Engine.Rules.Rules;

namespace Surveillance.Engine.Rules.Markets.Interfaces
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