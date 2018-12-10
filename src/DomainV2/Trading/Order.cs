using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Enums;
using DomainV2.Financial;

namespace DomainV2.Trading
{
    public class Order
    {
        public Order()
        {
            // used for extension method only
        }

        public Order(
            FinancialInstrument instrument,
            Market market,
            int? reddeerOrderId,
            string orderId,
            DateTime? orderPlacedDate,
            DateTime? orderBookedDate,
            DateTime? orderAmendedDate,
            DateTime? orderRejectedDate,
            DateTime? orderCancelledDate,
            DateTime? orderFilledDate,
            OrderTypes orderType,
            OrderPositions orderPosition,
            Currency orderCurrency,
            CurrencyAmount? orderLimitPrice,
            CurrencyAmount? orderAveragePrice,
            long? orderOrderedVolume,
            long? orderFilledVolume,
            string orderPortfolioManager,
            string orderTraderId,
            string orderExecutingBroker,
            string orderClearingAgent,
            string orderDealingInstructions,
            string orderStrategy,
            string orderRationale,
            string orderFund,
            string orderClientAccountAttributionId,
            IReadOnlyCollection<Trade> trades)
        {
            Instrument = instrument ?? throw new ArgumentNullException(nameof(instrument));
            Market = market ?? throw new ArgumentNullException(nameof(market));
            ReddeerOrderId = reddeerOrderId;
            OrderId = orderId ?? string.Empty;
            OrderPlacedDate = orderPlacedDate;
            OrderBookedDate = orderBookedDate;
            OrderAmendedDate = orderAmendedDate;
            OrderRejectedDate = orderRejectedDate;
            OrderCancelledDate = orderCancelledDate;
            OrderFilledDate = orderFilledDate;
            OrderType = orderType;
            OrderPosition = orderPosition;
            OrderCurrency = orderCurrency;
            OrderLimitPrice = orderLimitPrice;
            OrderAveragePrice = orderAveragePrice;
            OrderOrderedVolume = orderOrderedVolume;
            OrderFilledVolume = orderFilledVolume;
            OrderPortfolioManager = orderPortfolioManager ?? string.Empty;
            OrderTraderId = orderTraderId ?? string.Empty;
            OrderExecutingBroker = orderExecutingBroker ?? string.Empty;
            OrderClearingAgent = orderClearingAgent ?? string.Empty;
            OrderDealingInstructions = orderDealingInstructions ?? string.Empty;
            OrderStrategy = orderStrategy ?? string.Empty;
            OrderRationale = orderRationale ?? string.Empty;
            OrderFund = orderFund ?? string.Empty;
            OrderClientAccountAttributionId = orderClientAccountAttributionId ?? string.Empty;
            Trades = trades ?? new Trade[0];
        }

        public FinancialInstrument Instrument { get; set; }
        public Market Market { get; set; }

        public int? ReddeerOrderId { get; set; } // primary key
        public string OrderId { get; set; } // the client id for the order
        public DateTime? OrderPlacedDate { get; set; }
        public DateTime? OrderBookedDate { get; set; }
        public DateTime? OrderAmendedDate { get; set; }
        public DateTime? OrderRejectedDate { get; set; }
        public DateTime? OrderCancelledDate { get; set; }
        public DateTime? OrderFilledDate { get; set; }
        public OrderTypes OrderType { get; set; }
        public OrderPositions OrderPosition { get; set; }
        public Currency OrderCurrency { get; set; }
        public CurrencyAmount? OrderLimitPrice { get; set; }
        public CurrencyAmount? OrderAveragePrice { get; set; }
        public long? OrderOrderedVolume { get; set; }
        public long? OrderFilledVolume { get; set; }
        public string OrderPortfolioManager { get; set; }
        public string OrderTraderId { get; set; }
        public string OrderExecutingBroker { get; set; }
        public string OrderClearingAgent { get; set; }
        public string OrderDealingInstructions { get; set; }
        public string OrderStrategy { get; set; }
        public string OrderRationale { get; set; }
        public string OrderFund { get; set; }
        public string OrderClientAccountAttributionId { get; set; }
        public IReadOnlyCollection<Trade> Trades { get; set; }

        // Batch properties
        public bool IsInputBatch { get; set; }
        public int BatchSize { get; set; }
        public string InputBatchId { get; set; }

        public DateTime MostRecentDateEvent()
        {
            var dates = new[]
            {
                OrderPlacedDate,
                OrderBookedDate,
                OrderAmendedDate,
                OrderRejectedDate,
                OrderCancelledDate,
                OrderFilledDate
            };

            var filteredDates = dates.Where(dat => dat != null).ToList();
            if (!filteredDates.Any())
            {
                return DateTime.Now;
            }

            // placed should never be null i.e. this shouldn't call datetime.now
            return filteredDates.OrderByDescending(fd => fd).FirstOrDefault() ?? DateTime.Now; 
        }

        public OrderStatus OrderStatus()
        {
            if (OrderFilledDate != null)
            {
                return Financial.OrderStatus.Filled;
            }

            if (OrderCancelledDate != null)
            {
                return Financial.OrderStatus.Cancelled;
            }

            if (OrderRejectedDate != null)
            {
                return Financial.OrderStatus.Rejected;
            }

            if (OrderAmendedDate != null)
            {
                return Financial.OrderStatus.Amended;
            }

            if (OrderBookedDate != null)
            {
                return Financial.OrderStatus.Booked;
            }

            if (OrderPlacedDate != null)
            {
                return Financial.OrderStatus.Placed;
            }

            return Financial.OrderStatus.Unknown;
        }

        public override string ToString()
        {
            return $"{Instrument.Name} |{Market.Name} | {OrderStatus().GetDescription()} | ordered-{OrderOrderedVolume} | filled-{OrderFilledVolume} | {OrderAveragePrice?.ToString()}";
        }
    }
}
