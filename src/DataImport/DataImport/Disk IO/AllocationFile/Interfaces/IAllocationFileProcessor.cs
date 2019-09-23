namespace DataImport.Disk_IO.AllocationFile.Interfaces
{
    using Domain.Core.Trading.Orders;

    using SharedKernel.Files.Allocations;

    public interface IAllocationFileProcessor
    {
        UploadFileProcessorResult<AllocationFileContract, OrderAllocation> Process(string path);
    }
}