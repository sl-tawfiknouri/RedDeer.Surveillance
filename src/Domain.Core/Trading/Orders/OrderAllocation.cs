namespace Domain.Core.Trading.Orders
{
    using System;

    public class OrderAllocation
    {
        public OrderAllocation(
            string id,
            string orderId,
            string fund,
            string strategy,
            string clientAccountId,
            decimal orderFilledVolume,
            DateTime? createdDate)
        {
            this.Id = id ?? string.Empty;
            this.OrderId = orderId ?? string.Empty;
            this.Fund = fund ?? string.Empty;
            this.Strategy = strategy ?? string.Empty;
            this.ClientAccountId = clientAccountId ?? string.Empty;
            this.OrderFilledVolume = orderFilledVolume;
            this.CreatedDate = createdDate;
        }

        /// <summary>
        ///     100% allocation
        /// </summary>
        public OrderAllocation(Order order)
        {
            if (order == null) return;

            this.Id = string.Empty;
            this.OrderId = order.OrderId;
            this.Fund = order.OrderFund;
            this.Strategy = order.OrderStrategy;
            this.ClientAccountId = order.OrderClientAccountAttributionId;
            this.OrderFilledVolume = order.OrderFilledVolume.GetValueOrDefault(0);
        }

        public string ClientAccountId { get; }

        public DateTime? CreatedDate { get; }

        public string Fund { get; }

        public string Id { get; }

        public decimal OrderFilledVolume { get; }

        public string OrderId { get; }

        public string Strategy { get; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(this.OrderId);
        }
    }
}