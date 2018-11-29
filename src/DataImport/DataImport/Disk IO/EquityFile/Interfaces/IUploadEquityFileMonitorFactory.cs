using Domain.Equity.Streams.Interfaces;

namespace Relay.Disk_IO.EquityFile.Interfaces
{
    public interface IUploadEquityFileMonitorFactory
    {
        IUploadEquityFileMonitor Build(IStockExchangeStream exchangeStream);
    }
}