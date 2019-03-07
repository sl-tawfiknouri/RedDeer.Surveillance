using System;
using System.Collections.Generic;
using Domain.Core.Dates;
using Domain.Core.Financial;
using Domain.Core.Markets.Collections;
using Domain.Core.Markets.Timebars;
using SharedKernel.Contracts.Markets;
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