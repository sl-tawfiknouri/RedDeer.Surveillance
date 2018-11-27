using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Equity.Frames;
using Domain.Finance;
using Domain.Market;
using Domain.Trades.Orders;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators.Interfaces
{
    public interface IRevenueCalculator
    {
        Task<CurrencyAmount?> CalculateRevenueOfPosition(
            IList<TradeOrderFrame> activeFulfilledTradeOrders,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ctx,
            IDictionary<Market.MarketId, ExchangeFrame> latestExchangeFrameBook);
    }
}