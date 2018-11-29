using Relay.Disk_IO.Interfaces;

namespace Relay.Managers.Interfaces
{
    public interface ITradeOrderStreamManager
    {
        IUploadTradeFileMonitor Initialise();
    }
}