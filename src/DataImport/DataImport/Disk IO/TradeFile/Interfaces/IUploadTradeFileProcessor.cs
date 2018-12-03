using DomainV2.Files;
using DomainV2.Trading;

namespace DataImport.Disk_IO.TradeFile.Interfaces
{
    public interface IUploadTradeFileProcessor 
    {
        UploadFileProcessorResult<TradeFileCsv, Order> Process(string path);
    }
}