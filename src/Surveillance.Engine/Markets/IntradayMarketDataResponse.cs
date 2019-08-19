namespace Surveillance.Engine.Rules.Markets
{
    using System;

    using Domain.Core.Financial.Money;
    using Domain.Core.Markets.Timebars;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Engine.Rules.Markets.Interfaces;

    public class IntradayMarketDataResponse : IQueryableMarketDataResponse
    {
        private readonly MarketDataResponse<EquityInstrumentIntraDayTimeBar> _response;

        public IntradayMarketDataResponse(MarketDataResponse<EquityInstrumentIntraDayTimeBar> response)
        {
            this._response = response ?? throw new ArgumentNullException(nameof(response));
        }

        public bool HadMissingData()
        {
            return this._response.HadMissingData;
        }

        public Money? PriceOrClose()
        {
            return this._response?.Response?.SpreadTimeBar.Price;
        }
    }
}