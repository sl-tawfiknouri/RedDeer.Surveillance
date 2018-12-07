using DataImport.Disk_IO.Interfaces;

namespace DataImport.Managers.Interfaces
{
    public interface ITradeOrderStreamManager
    {
        IUploadTradeFileMonitor Initialise();
    }
}