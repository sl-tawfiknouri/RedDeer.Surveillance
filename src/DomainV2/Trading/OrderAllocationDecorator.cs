using System;

namespace DomainV2.Trading
{
    public class OrderAllocationDecorator : Order
    {
        private readonly OrderAllocation _orderAllocation;
        private readonly decimal _weighting;

        private readonly long? _baseOrderFilledVolume;
        private readonly long? _baseOrderOrderedVolume;

        public OrderAllocationDecorator(
            Order order, 
            OrderAllocation orderAllocation)
            : base(
                order.Instrument,
                order.Market,
                order.ReddeerOrderId,
                order.OrderId,
                order.OrderVersion,
                order.OrderVersionLinkId,
                order.OrderGroupId,
                order.OrderPlacedDate,
                order.OrderBookedDate,
                order.OrderAmendedDate,
                order.OrderRejectedDate,
                order.OrderCancelledDate,
                order.OrderFilledDate,
                order.OrderType,
                order.OrderDirection,
                order.OrderCurrency,
                order.OrderSettlementCurrency,
                order.OrderCleanDirty.GetValueOrDefault(Financial.OrderCleanDirty.NONE),
                order.OrderAccumulatedInterest,
                order.OrderLimitPrice,
                order.OrderAverageFillPrice,
                order.OrderOrderedVolume,
                order.OrderFilledVolume,
                order.OrderTraderId,
                order.OrderTraderName,
                order.OrderClearingAgent,
                order.OrderDealingInstructions,
                order.OrderOptionStrikePrice,
                order.OrderOptionExpirationDate,
                order.OrderOptionEuropeanAmerican,
                order.DealerOrders)
        {
            _orderAllocation = orderAllocation ?? throw new ArgumentNullException(nameof(orderAllocation));
            _baseOrderFilledVolume = order.OrderFilledVolume;
            _baseOrderOrderedVolume = order.OrderOrderedVolume;
            _weighting = OrderAllocationWeighting();
        }

        public override string OrderFund
        {
            get => _orderAllocation.Fund;
            set { }
        }

        public override string OrderStrategy
        {
            get => _orderAllocation.Strategy;
            set { }
        }

        public override string OrderClientAccountAttributionId
        {
            get => _orderAllocation.ClientAccountId;
            set { }
        }

        public override long? OrderFilledVolume
        {
            get => _orderAllocation.OrderFilledVolume;
            set { }
        }

        public override long? OrderOrderedVolume
        {
            get => (long?)(_baseOrderOrderedVolume * _weighting);
            set { }
        }

        private decimal OrderAllocationWeighting()
        {
            if (_orderAllocation.OrderFilledVolume == 0)
            {
                return 0;
            }

            if (!_baseOrderFilledVolume.HasValue
                || _baseOrderFilledVolume.Value == 0)
            {
                return 1;
            }

            if (_orderAllocation.OrderFilledVolume > _baseOrderFilledVolume)
            {
                return 1;
            }

            var weighting = (decimal)_orderAllocation.OrderFilledVolume / (decimal)_baseOrderFilledVolume;

            return weighting;
        }
    }
}
