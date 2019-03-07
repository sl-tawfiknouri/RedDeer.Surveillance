using Domain.Core.Trading.Orders;

namespace SharedKernel.Files.Allocations.Interfaces
{
    public interface IAllocationFileCsvToOrderAllocationSerialiser
    {
        OrderAllocation Map(AllocationFileContract contract);
        int FailedParseTotal { get; set; }
    }
}
