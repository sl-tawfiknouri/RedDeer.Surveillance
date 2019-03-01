using Domain.Trading;
using SharedKernel.Files.Orders;

namespace DataImport.Disk_IO.TradeFile.Interfaces
{
    public interface IUploadTradeFileProcessor 
    {
        UploadFileProcessorResult<OrderFileContract, Order> Process(string path);
    }
}