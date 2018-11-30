using System;
using Domain.Trades.Orders;

namespace DataImport.Recorders.Interfaces
{
    public interface IRedDeerTradeRecorder : IObserver<TradeOrderFrame>
    {
    }
}
