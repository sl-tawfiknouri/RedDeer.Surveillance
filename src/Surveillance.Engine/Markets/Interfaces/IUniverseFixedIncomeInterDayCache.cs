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
    public interface IUniverseFixedIncomeInterDayCache : ICloneable
    {
        void Add(FixedIncomeInterDayTimeBarCollection value);

        MarketDataResponse<FixedIncomeInstrumentInterDayTimeBar> Get(MarketDataRequest request);

        MarketDataResponse<List<FixedIncomeInstrumentInterDayTimeBar>> GetMarkets(MarketDataRequest request);

        MarketDataResponse<List<FixedIncomeInstrumentInterDayTimeBar>> GetMarketsForRange(
            MarketDataRequest request,
            IReadOnlyCollection<DateRange> dates,
            RuleRunMode runMode);
    }
}
