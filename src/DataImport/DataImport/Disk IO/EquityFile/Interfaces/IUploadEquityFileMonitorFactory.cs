using DomainV2.Equity.Streams.Interfaces;

namespace DataImport.Disk_IO.EquityFile.Interfaces
{
    public interface IUploadEquityFileMonitorFactory
    {
        IUploadEquityFileMonitor Build(IStockExchangeStream exchangeStream);
    }
}