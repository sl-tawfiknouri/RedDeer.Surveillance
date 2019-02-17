using Domain.Trading;

namespace Domain.Files.AllocationFile.Interfaces
{
    public interface IAllocationFileCsvToOrderAllocationMapper
    {
        OrderAllocation Map(AllocationFileCsv csv);
        int FailedParseTotal { get; set; }
    }
}
