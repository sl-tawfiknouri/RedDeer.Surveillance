using Domain.Trades.Orders;
using Domain.Trades.Streams.Interfaces;

namespace DataImport.Disk_IO.Interfaces
{
    public interface IUploadTradeFileMonitorFactory
    {
        IUploadTradeFileMonitor Create(ITradeOrderStream<TradeOrderFrame> stream);
    }
}