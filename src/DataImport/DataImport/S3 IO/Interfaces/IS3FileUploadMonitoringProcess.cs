using DataImport.Disk_IO.AllocationFile.Interfaces;
using DataImport.Disk_IO.EtlFile.Interfaces;
using DataImport.Disk_IO.Interfaces;

namespace DataImport.S3_IO.Interfaces
{
    public interface IS3FileUploadMonitoringProcess
    {
        void Initialise(
            IUploadAllocationFileMonitor uploadAllocationFileMonitor,
            IUploadTradeFileMonitor uploadTradeFileMonitor,
            IUploadEtlFileMonitor uploadEtlFileMonitor);

        void Terminate();
    }
}