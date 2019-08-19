namespace Domain.Core.Trading.Orders
{
    using System;

    public class OrderAllocationDecorator : Order
    {
        private readonly decimal? _baseOrderFilledVolume;

        private readonly decimal? _baseOrderOrderedVolume;

        private readonly OrderAllocation _orderAllocation;

        private readonly decimal _weighting;

        public OrderAllocationDecorator(Order order, OrderAllocation orderAllocation)
            : base(
                order.Instrument,
                order.Market,
                order.ReddeerOrderId,
                order.OrderId,
                order.CreatedDate,
                order.OrderVersion,
                order.OrderVersionLinkId,
                order.OrderGroupId,
                order.PlacedDate,
                order.BookedDate,
                order.AmendedDate,
                order.RejectedDate,
                order.CancelledDate,
                order.FilledDate,
                order.OrderType,
                order.OrderDirection,
                order.OrderCurrency,
                order.OrderSettlementCurrency,
                order.OrderCleanDirty.GetValueOrDefault(Orders.OrderCleanDirty.NONE),
                order.OrderAccumulatedInterest,
                order.OrderLimitPrice,
                order.OrderAverageFillPrice,
                order.OrderOrderedVolume,
                order.OrderFilledVolume,
                order.OrderTraderId,
                order.OrderTraderName,
                order.OrderClearingAgent,
                order.OrderDealingInstructions,
                order.OrderBroker,
                order.OrderOptionStrikePrice,
                order.OrderOptionExpirationDate,
                order.OrderOptionEuropeanAmerican,
                order.DealerOrders)
        {
            this._orderAllocation = orderAllocation ?? throw new ArgumentNullException(nameof(orderAllocation));
            this._baseOrderFilledVolume = order.OrderFilledVolume;
            this._baseOrderOrderedVolume = order.OrderOrderedVolume;
            this._weighting = this.OrderAllocationWeighting();
        }

        public override string OrderClientAccountAttributionId
        {
            get => this._orderAllocation.ClientAccountId;
            set
            {
            }
        }

        public override decimal? OrderFilledVolume
        {
            get => this._orderAllocation.OrderFilledVolume;
            set
            {
            }
        }

        public override string OrderFund
        {
            get => this._orderAllocation.Fund;
            set
            {
            }
        }

        public override decimal? OrderOrderedVolume
        {
            get => (long?)(this._baseOrderOrderedVolume * this._weighting);
            set
            {
            }
        }

        public override string OrderStrategy
        {
            get => this._orderAllocation.Strategy;
            set
            {
            }
        }

        private decimal OrderAllocationWeighting()
        {
            if (this._orderAllocation.OrderFilledVolume == 0) return 0;

            if (!this._baseOrderFilledVolume.HasValue || this._baseOrderFilledVolume.Value == 0) return 1;

            if (this._orderAllocation.OrderFilledVolume > this._baseOrderFilledVolume) return 1;

            var weighting = this._orderAllocation.OrderFilledVolume / (decimal)this._baseOrderFilledVolume;

            return weighting;
        }
    }
}