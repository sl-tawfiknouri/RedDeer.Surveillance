using Domain.Equity.Trading.Orders;
using System;

namespace Relay.Trades
{
    public interface ITradeProcessor : IObserver<TradeOrderFrame>, IObservable<TradeOrderFrame>
    {
    }
}
