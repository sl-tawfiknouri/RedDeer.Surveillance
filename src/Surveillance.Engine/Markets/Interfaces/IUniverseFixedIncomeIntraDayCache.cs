using Domain.Core.Dates;
using Domain.Core.Markets.Collections;
using Domain.Core.Markets.Timebars;
using SharedKernel.Contracts.Markets;
using Surveillance.Engine.Rules.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Engine.Rules.Markets.Interfaces
{
    public interface IUniverseFixedIncomeIntraDayCache : ICloneable
    {
        void Add(FixedIncomeIntraDayTimeBarCollection value);

        MarketDataResponse<FixedIncomeInstrumentIntraDayTimeBar> GetForLatestDayOnly(MarketDataRequest request);

        MarketDataResponse<List<FixedIncomeInstrumentIntraDayTimeBar>> GetMarkets(MarketDataRequest request);

        MarketDataResponse<List<FixedIncomeInstrumentIntraDayTimeBar>> GetMarketsForRange(
            MarketDataRequest request,
            IReadOnlyCollection<DateRange> dates,
            RuleRunMode runMode);
    }
}
