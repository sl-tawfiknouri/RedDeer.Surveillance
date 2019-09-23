namespace Domain.Core.Trading.Orders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Extensions;
    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Money;
    using Domain.Core.Markets;
    using Domain.Core.Trading.Orders.Interfaces;

    /// <summary>
    ///     This is the Order from within the firm originating the order
    ///     It has associations with dealer orders which are orders
    ///     as processed by the firm dealing the trades
    /// </summary>
    public class Order : BaseOrder
    {
        public Order()
            : base(null, null, null, null, null, null)
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
            decimal? orderOrderedVolume,
            decimal? orderFilledVolume,
            string orderTraderId,
            string orderTraderName,
            string orderClearingAgent,
            string orderDealingInstructions,
            IOrderBroker orderBroker,
            Money? orderOptionStrikePrice,
            DateTime? orderOptionExpirationDate,
            OptionEuropeanAmerican orderOptionEuropeanAmerican,
            IReadOnlyCollection<DealerOrder> trades)
            : base(placedDate, bookedDate, amendedDate, rejectedDate, cancelledDate, filledDate)
        {
            // keys
            this.Instrument = instrument ?? throw new ArgumentNullException(nameof(instrument));
            this.Market = market ?? throw new ArgumentNullException(nameof(market));
            this.ReddeerOrderId = reddeerOrderId;
            this.OrderId = orderId ?? string.Empty;

            // versioning
            this.OrderVersion = orderVersion ?? string.Empty;
            this.OrderVersionLinkId = orderVersionLinkId ?? string.Empty;
            this.OrderGroupId = orderGroupId ?? string.Empty;

            // dates
            this.CreatedDate = created;

            // order fundamentals
            this.OrderType = orderType;
            this.OrderDirection = orderDirection;
            this.OrderCurrency = orderCurrency;
            this.OrderSettlementCurrency = orderSettlementCurrency;
            this.OrderCleanDirty = orderCleanDirty;
            this.OrderAccumulatedInterest = orderAccumulatedInterest;
            this.OrderLimitPrice = orderLimitPrice;
            this.OrderAverageFillPrice = orderAverageFillPrice;
            this.OrderOrderedVolume = orderOrderedVolume;
            this.OrderFilledVolume = orderFilledVolume;
            this.OrderBroker = orderBroker;

            // order trader and post trade
            this.OrderTraderId = orderTraderId ?? string.Empty;
            this.OrderTraderName = orderTraderName ?? string.Empty;
            this.OrderClearingAgent = orderClearingAgent ?? string.Empty;
            this.OrderDealingInstructions = orderDealingInstructions ?? string.Empty;

            // options
            this.OrderOptionStrikePrice = orderOptionStrikePrice;
            this.OrderOptionExpirationDate = orderOptionExpirationDate;
            this.OrderOptionEuropeanAmerican = orderOptionEuropeanAmerican;

            // associated dealer orders
            this.DealerOrders = trades ?? new DealerOrder[0];
        }

        public int BatchSize { get; set; }

        public DateTime? CreatedDate { get; set; }

        public IReadOnlyCollection<DealerOrder> DealerOrders { get; set; }

        public string InputBatchId { get; set; }

        public FinancialInstrument Instrument { get; set; }

        // Batch properties
        public bool IsInputBatch { get; set; }

        public Market Market { get; set; }

        public decimal? OrderAccumulatedInterest { get; set; }

        public Money? OrderAverageFillPrice { get; set; }

        public IOrderBroker OrderBroker { get; set; }

        public OrderCleanDirty? OrderCleanDirty { get; set; }

        public string OrderClearingAgent { get; set; }

        public virtual string OrderClientAccountAttributionId { get; set; } = string.Empty;

        public Currency OrderCurrency { get; set; }

        public string OrderDealingInstructions { get; set; }

        public OrderDirections OrderDirection { get; set; }

        public virtual decimal? OrderFilledVolume { get; set; }

        // Accounting allocation Properties
        public virtual string OrderFund { get; set; } = string.Empty;

        public string OrderGroupId { get; set; }

        public string OrderId { get; set; } // the client id for the order

        public Money? OrderLimitPrice { get; set; }

        public OptionEuropeanAmerican OrderOptionEuropeanAmerican { get; set; }

        public DateTime? OrderOptionExpirationDate { get; set; }

        // Options
        public Money? OrderOptionStrikePrice { get; set; }

        // can be overridden by accounting allocations
        public virtual decimal? OrderOrderedVolume { get; set; }

        public Currency? OrderSettlementCurrency { get; set; }

        public virtual string OrderStrategy { get; set; } = string.Empty;

        public string OrderTraderId { get; set; }

        public string OrderTraderName { get; set; }

        public OrderTypes OrderType { get; set; }

        public string OrderVersion { get; set; }

        public string OrderVersionLinkId { get; set; }

        public int? ReddeerOrderId { get; set; } // primary key for the order

        public DateTime MostRecentDateEvent()
        {
            var dates = new[]
                            {
                                this.PlacedDate, this.BookedDate, this.AmendedDate, this.RejectedDate,
                                this.CancelledDate, this.FilledDate
                            };

            var filteredDates = dates.Where(dat => dat != null).ToList();
            if (!filteredDates.Any()) return DateTime.UtcNow;

            // placed should never be null i.e. this shouldn't call datetime.now
            return filteredDates.OrderByDescending(fd => fd).FirstOrDefault() ?? DateTime.UtcNow;
        }

        public override string ToString()
        {
            return
                $"{this.Instrument.Name} |{this.Market.Name} | {this.OrderStatus().GetDescription()} | ordered-{this.OrderOrderedVolume} | filled-{this.OrderFilledVolume} | {this.OrderAverageFillPrice?.ToString()}";
        }
    }
}