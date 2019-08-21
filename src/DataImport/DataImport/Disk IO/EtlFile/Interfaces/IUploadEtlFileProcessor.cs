namespace DataImport.Disk_IO.EtlFile.Interfaces
{
    using Domain.Core.Trading.Orders;

    using SharedKernel.Files.Orders;

    public interface IUploadEtlFileProcessor
    {
        UploadFileProcessorResult<OrderFileContract, Order> Process(string path);
    }
}