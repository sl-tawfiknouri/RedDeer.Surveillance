using Relay.Disk_IO.EquityFile.Interfaces;
using Relay.Disk_IO.Interfaces;

namespace Relay.S3_IO.Interfaces
{
    public interface IS3FileUploadMonitoringProcess
    {
        void Initialise(
            IUploadTradeFileMonitor uploadTradeFileMonitor,
            IUploadEquityFileMonitor uploadEquityFileMonitor);
        void Terminate();
    }
}