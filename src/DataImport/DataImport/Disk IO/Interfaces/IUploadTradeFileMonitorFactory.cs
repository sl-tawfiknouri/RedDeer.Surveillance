using DomainV2.Streams;
using DomainV2.Trading;

namespace DataImport.Disk_IO.Interfaces
{
    public interface IUploadTradeFileMonitorFactory
    {
        IUploadTradeFileMonitor Create(OrderStream<Order> stream);
    }
}