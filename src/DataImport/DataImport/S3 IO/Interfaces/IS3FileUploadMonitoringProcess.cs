using DataImport.Disk_IO.EquityFile.Interfaces;
using DataImport.Disk_IO.Interfaces;

namespace DataImport.S3_IO.Interfaces
{
    public interface IS3FileUploadMonitoringProcess
    {
        void Initialise(
            IUploadTradeFileMonitor uploadTradeFileMonitor,
            IUploadEquityFileMonitor uploadEquityFileMonitor);
        void Terminate();
    }
}