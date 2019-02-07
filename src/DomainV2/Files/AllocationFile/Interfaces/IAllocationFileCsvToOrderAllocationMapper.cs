using DomainV2.Trading;

namespace DomainV2.Files.AllocationFile.Interfaces
{
    public interface IAllocationFileCsvToOrderAllocationMapper
    {
        OrderAllocation Map(AllocationFileCsv csv);
        int FailedParseTotal { get; set; }
    }
}
