using Domain.Equity.Trading.Orders;
using System;

namespace Surveillance.Recorders.Interfaces
{
    public interface IRedDeerTradeRecorder : IObserver<TradeOrderFrame>
    {
    }
}
