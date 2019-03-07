using System;
using Domain.Core.Financial.Money;
using Domain.Core.Markets.Timebars;
using SharedKernel.Contracts.Markets;
using Surveillance.Engine.Rules.Markets.Interfaces;

namespace Surveillance.Engine.Rules.Markets
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

        public Money? PriceOrClose()
        {
            return _response?.Response?.SpreadTimeBar.Price;
        }
    }
}
