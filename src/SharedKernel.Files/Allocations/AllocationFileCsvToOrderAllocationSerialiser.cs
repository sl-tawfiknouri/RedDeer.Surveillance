using System;
using Domain.Core.Trading.Orders;
using SharedKernel.Files.Allocations.Interfaces;

namespace SharedKernel.Files.Allocations
{
    public class AllocationFileCsvToOrderAllocationSerialiser : IAllocationFileCsvToOrderAllocationSerialiser
    {
        public int FailedParseTotal { get; set; }

        public OrderAllocation Map(AllocationFileContract contract)
        {
            if (contract == null)
            {
                FailedParseTotal += 1;
                return null;
            }

            return MapAllocation(contract);    
        }

        private OrderAllocation MapAllocation(AllocationFileContract contract)
        {
            var orderFilledVolume = MapDecimal(contract.OrderFilledVolume);

            var allocation = new OrderAllocation(
                null,
                contract.OrderId,
                contract.Fund,
                contract.Strategy,
                contract.ClientAccountId,
                orderFilledVolume.GetValueOrDefault(0),
                DateTime.UtcNow);

            return allocation;
        }

        private decimal? MapDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var success = decimal.TryParse(value, out var result);

            if (success)
                return result;

            return null;
        }
    }
}
