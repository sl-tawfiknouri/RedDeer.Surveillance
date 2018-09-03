using System;
using Domain.Trades.Orders;

namespace Surveillance.Recorders.Interfaces
{
    public interface IRedDeerTradeRecorder : IObserver<TradeOrderFrame>
    {
    }
}
