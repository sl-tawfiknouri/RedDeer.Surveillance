using System.Collections.Generic;
using Domain.Trades.Orders;

namespace Relay.Disk_IO.Interfaces
{
    public interface IUploadTradeFileProcessor
    {
        IReadOnlyCollection<TradeOrderFrame> Process(string path);
    }
}