using DomainV2.Files;
using DomainV2.Trading;

namespace DataImport.Disk_IO.AllocationFile.Interfaces
{
    public interface IAllocationFileProcessor
    {
        UploadFileProcessorResult<AllocationFileCsv, OrderAllocation> Process(string path);
    }
}
