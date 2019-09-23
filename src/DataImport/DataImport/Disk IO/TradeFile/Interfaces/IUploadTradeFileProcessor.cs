namespace DataImport.Disk_IO.TradeFile.Interfaces
{
    using Domain.Core.Trading.Orders;

    using SharedKernel.Files.Orders;

    public interface IUploadTradeFileProcessor
    {
        UploadFileProcessorResult<OrderFileContract, Order> Process(string path);
    }
}