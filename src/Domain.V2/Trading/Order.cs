using System;
using Domain.V2.Financial;

namespace Domain.V2.Trading
{
    public class Order
    {
        public Order(
            FinancialInstrument instrument,
            string reddeerOrderId,
            string orderId,
            DateTime? orderPlacedDate,
            DateTime? orderBookedDate,
            DateTime? orderAmendedDate,
            DateTime? orderRejectedDate,
            DateTime? orderCancelledDate,
            DateTime? orderFilledDate,
            string orderType,
            string orderPosition,
            string orderCurrency,
            decimal? orderLimitPrice,
            decimal? orderAveragePrice,
            long? orderOrderedVolume,
            long? orderFilledVolume,
            string orderPortfolioManager,
            string orderExecutingBroker,
            string orderClearingAgent,
            string orderDealingInstructions,
            string orderStrategy,
            string orderRationale,
            string orderFund,
            string orderClientAccountAttributionId)
        {
            Instrument = instrument ?? throw new ArgumentNullException(nameof(instrument));
            ReddeerOrderId = reddeerOrderId ?? string.Empty;
            OrderId = orderId ?? string.Empty;
            OrderPlacedDate = orderPlacedDate;
            OrderBookedDate = orderBookedDate;
            OrderAmendedDate = orderAmendedDate;
            OrderRejectedDate = orderRejectedDate;
            OrderCancelledDate = orderCancelledDate;
            OrderFilledDate = orderFilledDate;
            OrderType = orderType ?? string.Empty;
            OrderPosition = orderPosition ?? string.Empty;
            OrderCurrency = orderCurrency ?? string.Empty;
            OrderLimitPrice = orderLimitPrice;
            OrderAveragePrice = orderAveragePrice;
            OrderOrderedVolume = orderOrderedVolume;
            OrderFilledVolume = orderFilledVolume;
            OrderPortfolioManager = orderPortfolioManager ?? string.Empty;
            OrderExecutingBroker = orderExecutingBroker ?? string.Empty;
            OrderClearingAgent = orderClearingAgent ?? string.Empty;
            OrderDealingInstructions = orderDealingInstructions ?? string.Empty;
            OrderStrategy = orderStrategy ?? string.Empty;
            OrderRationale = orderRationale ?? string.Empty;
            OrderFund = orderFund ?? string.Empty;
            OrderClientAccountAttributionId = orderClientAccountAttributionId ?? string.Empty;
        }

        public FinancialInstrument Instrument { get; }

        public string ReddeerOrderId { get; }
        public string OrderId { get; } // the client id for the order
        public DateTime? OrderPlacedDate { get; }
        public DateTime? OrderBookedDate { get; }
        public DateTime? OrderAmendedDate { get; }
        public DateTime? OrderRejectedDate { get; }
        public DateTime? OrderCancelledDate { get; }
        public DateTime? OrderFilledDate { get; }
        public string OrderType { get; }
        public string OrderPosition { get; }
        public string OrderCurrency { get; }
        public decimal? OrderLimitPrice { get; }
        public decimal? OrderAveragePrice { get; }
        public long? OrderOrderedVolume { get; }
        public long? OrderFilledVolume { get; }
        public string OrderPortfolioManager { get; }
        public string OrderExecutingBroker { get; }
        public string OrderClearingAgent { get; }
        public string OrderDealingInstructions { get; }
        public string OrderStrategy { get; }
        public string OrderRationale { get; }
        public string OrderFund { get; }
        public string OrderClientAccountAttributionId { get; }
    }
}
