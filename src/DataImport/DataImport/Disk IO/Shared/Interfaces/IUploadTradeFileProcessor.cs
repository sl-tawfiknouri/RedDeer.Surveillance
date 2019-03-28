using Domain.Core.Trading.Orders;
using SharedKernel.Files.Orders;

namespace DataImport.Disk_IO.Shared.Interfaces
{
    public interface IUploadTradeFileProcessor 
    {
        UploadFileProcessorResult<OrderFileContract, Order> Process(string path);
    }
}