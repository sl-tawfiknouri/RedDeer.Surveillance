using DataImport.Disk_IO.EquityFile.Interfaces;

namespace DataImport.Managers.Interfaces
{
    public interface IStockExchangeStreamManager
    {
        IUploadEquityFileMonitor Initialise();
    }
}