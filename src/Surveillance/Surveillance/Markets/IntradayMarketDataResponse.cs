using System;
using DomainV2.Equity.TimeBars;
using DomainV2.Financial;
using DomainV2.Markets;
using Surveillance.Markets.Interfaces;

namespace Surveillance.Markets
{
    public class IntradayMarketDataResponse : IQueryableMarketDataResponse
    {
        private readonly MarketDataResponse<EquityInstrumentIntraDayTimeBar> _response;

        public IntradayMarketDataResponse(MarketDataResponse<EquityInstrumentIntraDayTimeBar> response)
        {
            _response = response ?? throw new ArgumentNullException(nameof(response));
        }

        public bool HadMissingData()
        {
            return _response.HadMissingData;
        }

        public CurrencyAmount? PriceOrClose()
        {
            return _response?.Response?.SpreadTimeBar.Price;
        }
    }
}
