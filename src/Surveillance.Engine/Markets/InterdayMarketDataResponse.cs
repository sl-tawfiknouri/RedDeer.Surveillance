﻿using System;
using Domain.Equity.TimeBars;
using Domain.Financial;
using Domain.Markets;
using Surveillance.Engine.Rules.Markets.Interfaces;

namespace Surveillance.Engine.Rules.Markets
{
    public class InterdayMarketDataResponse : IQueryableMarketDataResponse
    {
        private readonly MarketDataResponse<EquityInstrumentInterDayTimeBar> _response;

        public InterdayMarketDataResponse(MarketDataResponse<EquityInstrumentInterDayTimeBar> response)
        {
            _response = response ?? throw new ArgumentNullException(nameof(response));
        }

        public bool HadMissingData()
        {
            return _response.HadMissingData;
        }

        public CurrencyAmount? PriceOrClose()
        {
            return _response?.Response?.DailySummaryTimeBar?.IntradayPrices?.Close;
        }
    }
}