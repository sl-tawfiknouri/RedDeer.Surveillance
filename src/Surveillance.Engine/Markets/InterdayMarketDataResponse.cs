using System;
using Domain.Core.Financial;
using Domain.Core.Financial.Money;
using Domain.Core.Markets.Timebars;
using SharedKernel.Contracts.Markets;
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

        public Money? PriceOrClose()
        {
            return _response?.Response?.DailySummaryTimeBar?.IntradayPrices?.Close;
        }
    }
}
