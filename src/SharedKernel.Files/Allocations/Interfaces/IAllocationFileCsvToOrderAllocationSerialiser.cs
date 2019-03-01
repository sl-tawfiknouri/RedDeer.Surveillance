using Domain.Trading;

namespace SharedKernel.Files.Allocations.Interfaces
{
    public interface IAllocationFileCsvToOrderAllocationSerialiser
    {
        OrderAllocation Map(AllocationFileContract contract);
        int FailedParseTotal { get; set; }
    }
}
