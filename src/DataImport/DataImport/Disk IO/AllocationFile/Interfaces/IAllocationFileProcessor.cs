using Domain.Trading;
using SharedKernel.Files.Allocations;

namespace DataImport.Disk_IO.AllocationFile.Interfaces
{
    public interface IAllocationFileProcessor
    {
        UploadFileProcessorResult<AllocationFileContract, OrderAllocation> Process(string path);
    }
}
