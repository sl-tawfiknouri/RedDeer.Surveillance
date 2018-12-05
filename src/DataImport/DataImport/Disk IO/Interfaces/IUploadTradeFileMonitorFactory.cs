using Domain.Trades.Streams.Interfaces;
using DomainV2.Trading;

namespace DataImport.Disk_IO.Interfaces
{
    public interface IUploadTradeFileMonitorFactory
    {
        IUploadTradeFileMonitor Create(ITradeOrderStream<Order> stream);
    }
}