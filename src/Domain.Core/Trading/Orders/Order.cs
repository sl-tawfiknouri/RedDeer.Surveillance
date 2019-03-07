using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Extensions;
using Domain.Core.Financial.Assets;
using Domain.Core.Financial.Money;
using Domain.Core.Markets;

namespace Domain.Core.Trading.Orders
{
    /// <summary>
    /// This is the Order from within the firm originating the order
    /// It has associations with dealer orders which are orders
    /// as processed by the firm dealing the trades
    /// </summary>
    public class Order : BaseOrder
    {
        public Order() : base(null, null, null, null, null, null)
        {
            // used for extension method only
        }

        public Order(
            FinancialInstrument instrument,
            Market market,
            int? reddeerOrderId,
            string orderId,
            DateTime? created,
            
            string orderVersion,
            string orderVersionLinkId,
            string orderGroupId,

            DateTime? placedDate,
            DateTime? bookedDate,
            DateTime? amendedDate,
            DateTime? rejectedDate,
            DateTime? cancelledDate,
            DateTime? filledDate,

            OrderTypes orderType,
            OrderDirections orderDirection,
            Currency orderCurrency,
            Currency? orderSettlementCurrency,
            OrderCleanDirty orderCleanDirty,
            decimal? orderAccumulatedInterest,

            Money? orderLimitPrice,
            Money? orderAverageFillPrice,
            long? orderOrderedVolume,
            long? orderFilledVolume,
            string orderTraderId,
            string orderTraderName,
            string orderClearingAgent,
            string orderDealingInstructions,

            Money? orderOptionStrikePrice,
            DateTime? orderOptionExpirationDate,
            OptionEuropeanAmerican orderOptionEuropeanAmerican,
            
            IReadOnlyCollection<DealerOrder> trades)
            : base(
                placedDate,
                bookedDate,
                amendedDate,
                rejectedDate,
                cancelledDate,
                filledDate)
        {
            // keys
            Instrument = instrument ?? throw new ArgumentNullException(nameof(instrument));
            Market = market ?? throw new ArgumentNullException(nameof(market));
            ReddeerOrderId = reddeerOrderId;
            OrderId = orderId ?? string.Empty;

            // versioning
            OrderVersion = orderVersion ?? string.Empty;
            OrderVersionLinkId = orderVersionLinkId ?? string.Empty;
            OrderGroupId = orderGroupId ?? string.Empty;

            // dates
            CreatedDate = created;

            // order fundamentals
            OrderType = orderType;
            OrderDirection = orderDirection;
            OrderCurrency = orderCurrency;
            OrderSettlementCurrency = orderSettlementCurrency;
            OrderCleanDirty = orderCleanDirty;
            OrderAccumulatedInterest = orderAccumulatedInterest;
            OrderLimitPrice = orderLimitPrice;
            OrderAverageFillPrice = orderAverageFillPrice;
            OrderOrderedVolume = orderOrderedVolume;
            OrderFilledVolume = orderFilledVolume;

            // order trader and post trade
            OrderTraderId = orderTraderId ?? string.Empty;
            OrderTraderName = orderTraderName ?? string.Empty;
            OrderClearingAgent = orderClearingAgent ?? string.Empty;
            OrderDealingInstructions = orderDealingInstructions ?? string.Empty;

            // options
            OrderOptionStrikePrice = orderOptionStrikePrice;
            OrderOptionExpirationDate = orderOptionExpirationDate;
            OrderOptionEuropeanAmerican = orderOptionEuropeanAmerican;

            // associated dealer orders
            DealerOrders = trades ?? new DealerOrder[0];
        }

        public FinancialInstrument Instrument { get; set; }
        public Market Market { get; set; }

        public int? ReddeerOrderId { get; set; } // primary key for the order
        public string OrderId { get; set; } // the client id for the order

        public string OrderVersion { get; set; }
        public string OrderVersionLinkId { get; set; }
        public string OrderGroupId { get; set; }

        // Options
        public Money? OrderOptionStrikePrice { get; set; }
        public DateTime? OrderOptionExpirationDate { get; set; }
        public OptionEuropeanAmerican OrderOptionEuropeanAmerican { get; set; }


        public DateTime? CreatedDate { get; set; }


        public OrderTypes OrderType { get; set; }
        public OrderDirections OrderDirection { get; set; }
        public Currency OrderCurrency { get; set; }
        public Currency? OrderSettlementCurrency { get; set; }
        public OrderCleanDirty? OrderCleanDirty { get; set; }
        public decimal? OrderAccumulatedInterest { get; set; }

        public Money? OrderLimitPrice { get; set; }
        public Money? OrderAverageFillPrice { get; set; }


        public string OrderTraderId { get; set; }
        public string OrderTraderName { get; set; }
        public string OrderClearingAgent { get; set; }
        public string OrderDealingInstructions { get; set; }


        public IReadOnlyCollection<DealerOrder> DealerOrders { get; set; }
        
        // can be overridden by accounting allocations
        public virtual long? OrderOrderedVolume { get; set; }
        public virtual long? OrderFilledVolume { get; set; }
        
        // Accounting allocation Properties
        public virtual string OrderFund { get; set; } = string.Empty;
        public virtual string OrderStrategy { get; set; } = string.Empty;
        public virtual string OrderClientAccountAttributionId { get; set; } = string.Empty;


        // Batch properties
        public bool IsInputBatch { get; set; }
        public int BatchSize { get; set; }
        public string InputBatchId { get; set; }

        public DateTime MostRecentDateEvent()
        {
            var dates = new[]
            {
                PlacedDate,
                BookedDate,
                AmendedDate,
                RejectedDate,
                CancelledDate,
                FilledDate
            };

            var filteredDates = dates.Where(dat => dat != null).ToList();
            if (!filteredDates.Any())
            {
                return DateTime.UtcNow;
            }

            // placed should never be null i.e. this shouldn't call datetime.now
            return filteredDates.OrderByDescending(fd => fd).FirstOrDefault() ?? DateTime.UtcNow; 
        }

        public override string ToString()
        {
            return $"{Instrument.Name} |{Market.Name} | {OrderStatus().GetDescription()} | ordered-{OrderOrderedVolume} | filled-{OrderFilledVolume} | {OrderAverageFillPrice?.ToString()}";
        }
    }
}
