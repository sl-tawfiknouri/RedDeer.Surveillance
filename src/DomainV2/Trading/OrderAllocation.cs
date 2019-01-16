namespace DomainV2.Trading
{
    public class OrderAllocation
    {
        public OrderAllocation(
            string id,
            string orderId,
            string fund,
            string strategy,
            string clientAccountId,
            long orderFilledVolume)
        {
            Id = id ?? string.Empty;
            OrderId = orderId ?? string.Empty;
            Fund = fund ?? string.Empty;
            Strategy = strategy ?? string.Empty;
            ClientAccountId = clientAccountId ?? string.Empty;
            OrderFilledVolume = orderFilledVolume;
        }

        public string Id { get; }
        public string OrderId { get; }
        public string Fund { get; }
        public string Strategy { get; }
        public string ClientAccountId { get; }
        public long OrderFilledVolume { get; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(OrderId);
        }
    }
}
