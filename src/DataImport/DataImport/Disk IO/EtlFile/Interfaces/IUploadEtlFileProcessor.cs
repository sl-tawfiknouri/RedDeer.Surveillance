using Domain.Core.Trading.Orders;
using SharedKernel.Files.Orders;

namespace DataImport.Disk_IO.EtlFile.Interfaces
{
    public interface IUploadEtlFileProcessor
    {
         UploadFileProcessorResult<OrderFileContract, Order> Process(string path);
    }
}
