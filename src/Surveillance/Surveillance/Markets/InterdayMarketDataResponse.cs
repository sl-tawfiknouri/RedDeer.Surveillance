using System;
using DomainV2.Equity.TimeBars;
using DomainV2.Financial;
using DomainV2.Markets;
using Surveillance.Markets.Interfaces;

namespace Surveillance.Markets
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
