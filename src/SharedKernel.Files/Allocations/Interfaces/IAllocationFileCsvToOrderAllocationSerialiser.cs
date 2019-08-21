namespace SharedKernel.Files.Allocations.Interfaces
{
    using Domain.Core.Trading.Orders;

    public interface IAllocationFileCsvToOrderAllocationSerialiser
    {
        int FailedParseTotal { get; set; }

        OrderAllocation Map(AllocationFileContract contract);
    }
}