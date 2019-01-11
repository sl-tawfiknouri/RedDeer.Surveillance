using DataImport.Disk_IO.Interfaces;
using DomainV2.Equity.TimeBars;

namespace DataImport.Disk_IO.EquityFile.Interfaces
{
    public interface IUploadEquityFileProcessor : IBaseUploadFileProcessor<FinancialInstrumentTimeBarCsv, MarketTimeBarCollection>
    {
    }
}