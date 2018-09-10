using System;
using Domain.Trades.Orders;

namespace Surveillance.Rules.Interfaces
{
    public interface ITradeRule : IObserver<TradeOrderFrame>
    {
        Rules Rule { get; }
        string Version { get; }
    }
}
