using DomainV2.Streams.Interfaces;
using DomainV2.Trading;

namespace DataImport.Disk_IO.AllocationFile
{
    public interface IAllocationFileMonitorFactory
    {
        IUploadAllocationFileMonitor Create(IOrderAllocationStream<OrderAllocation> stream);
    }
}