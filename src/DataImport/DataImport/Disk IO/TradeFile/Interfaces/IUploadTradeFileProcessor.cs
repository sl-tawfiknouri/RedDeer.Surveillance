using Domain.Trades.Orders;

namespace DataImport.Disk_IO.TradeFile.Interfaces
{
    public interface IUploadTradeFileProcessor 
    {
        UploadFileProcessorResult<TradeOrderFrameCsv, TradeOrderFrame> Process(string path);
    }
}