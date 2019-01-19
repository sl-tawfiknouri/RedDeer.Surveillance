﻿using System;
using System.Collections.Generic;
using DomainV2.Equity.TimeBars;
using DomainV2.Markets;

namespace Surveillance.Markets.Interfaces
{
    public interface IUniverseEquityIntradayCache : ICloneable
    {
        void Add(EquityIntraDayTimeBarCollection value);
        MarketDataResponse<EquityInstrumentIntraDayTimeBar> Get(MarketDataRequest request);
        MarketDataResponse<List<EquityInstrumentIntraDayTimeBar>> GetMarkets(MarketDataRequest request);
    }
}