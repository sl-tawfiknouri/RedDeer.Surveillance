using Relay.Disk_IO.EquityFile.Interfaces;

namespace Relay.Managers.Interfaces
{
    public interface IStockExchangeStreamManager
    {
        IUploadEquityFileMonitor Initialise();
    }
}