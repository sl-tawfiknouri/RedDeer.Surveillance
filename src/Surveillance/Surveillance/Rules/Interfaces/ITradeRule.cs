using Domain.Equity.Trading.Orders;
using System;

namespace Surveillance.Rules.Interfaces
{
    public interface ITradeRule : IObserver<TradeOrderFrame>
    {
    }
}
