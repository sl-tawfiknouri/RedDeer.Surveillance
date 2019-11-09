using Domain.Core.Financial.Money;
using Domain.Core.Markets.Timebars;
using SharedKernel.Contracts.Markets;
using Surveillance.Engine.Rules.Markets.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Engine.Rules.Markets
{
    public class FixedIncomeInterDayMarketDataResponse : IQueryableMarketDataResponse
    {
        private readonly MarketDataResponse<FixedIncomeInstrumentInterDayTimeBar> _response;

        public FixedIncomeInterDayMarketDataResponse(MarketDataResponse<FixedIncomeInstrumentInterDayTimeBar> response)
        {
            this._response = response ?? throw new ArgumentNullException(nameof(response));
        }

        public bool HadMissingData()
        {
            return this._response.HadMissingData;
        }

        public Money? PriceOrClose()
        {
            return this._response?.Response?.DailySummaryTimeBar?.IntradayPrices?.Close;
        }
    }
}
