using Domain.Files.AllocationFile;
using Domain.Trading;

namespace DataImport.Disk_IO.AllocationFile.Interfaces
{
    public interface IAllocationFileProcessor
    {
        UploadFileProcessorResult<AllocationFileCsv, OrderAllocation> Process(string path);
    }
}
