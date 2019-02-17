using Domain.Files;
using Domain.Trading;

namespace DataImport.Disk_IO.TradeFile.Interfaces
{
    public interface IUploadTradeFileProcessor 
    {
        UploadFileProcessorResult<TradeFileCsv, Order> Process(string path);
    }
}