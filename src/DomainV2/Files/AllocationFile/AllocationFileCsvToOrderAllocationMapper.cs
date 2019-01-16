using DomainV2.Files.AllocationFile.Interfaces;
using DomainV2.Trading;

namespace DomainV2.Files.AllocationFile
{
    public class AllocationFileCsvToOrderAllocationMapper : IAllocationFileCsvToOrderAllocationMapper
    {
        public int FailedParseTotal { get; set; }

        public OrderAllocation Map(AllocationFileCsv csv)
        {
            if (csv == null)
            {
                FailedParseTotal += 1;
                return null;
            }

            return MapAllocation(csv);    
        }

        private OrderAllocation MapAllocation(AllocationFileCsv csv)
        {
            var orderFilledVolume = MapLong(csv.OrderFilledVolume);

            var allocation = new OrderAllocation(
                null,
                csv.OrderId,
                csv.Fund,
                csv.Strategy,
                csv.ClientAccountId,
                orderFilledVolume.GetValueOrDefault(0));

            return allocation;
        }

        private long? MapLong(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var success = long.TryParse(value, out var result);

            if (success)
                return result;

            return null;
        }
    }
}
