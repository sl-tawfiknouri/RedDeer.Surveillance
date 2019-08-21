namespace Domain.Core.Trading.Orders
{
    using System;
    using System.Linq;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Assets.Interfaces;
    using Domain.Core.Financial.Money;

    public class DealerOrder : BaseOrder
    {
        public DealerOrder(
            IFinancialInstrument instrument,
            string reddeerTradeId,
            string tradeId,
            DateTime? placedDate,
            DateTime? bookedDate,
            DateTime? amendedDate,
            DateTime? rejectedDate,
            DateTime? cancelledDate,
            DateTime? filledDate,
            DateTime? createdDate,
            string traderId,
            string dealerName,
            string notes,
            string tradeCounterParty,
            OrderTypes orderType,
            OrderDirections orderDirection,
            Currency currency,
            Currency settlementCurrency,
            OrderCleanDirty cleanDirty,
            decimal? accumulatedInterest,
            string dealerOrderVersion,
            string dealerOrderVersionLinkId,
            string dealerOrderGroupId,
            Money? limitPrice,
            Money? averageFillPrice,
            decimal? orderedVolume,
            decimal? filledVolume,
            decimal? optionStrikePrice,
            DateTime? optionExpirationDate,
            OptionEuropeanAmerican tradeOptionEuropeanAmerican)
            : base(placedDate, bookedDate, amendedDate, rejectedDate, cancelledDate, filledDate)
        {
            this.Instrument = instrument ?? throw new ArgumentNullException(nameof(instrument));
            this.ReddeerDealerOrderId = reddeerTradeId ?? string.Empty;
            this.DealerOrderId = tradeId ?? string.Empty;

            this.CreatedDate = createdDate;

            this.DealerId = traderId ?? string.Empty;
            this.DealerName = dealerName ?? string.Empty;
            this.Notes = notes ?? string.Empty;
            this.DealerCounterParty = tradeCounterParty ?? string.Empty;
            this.OrderType = orderType;
            this.OrderDirection = orderDirection;
            this.Currency = currency;
            this.SettlementCurrency = settlementCurrency;
            this.CleanDirty = cleanDirty;
            this.AccumulatedInterest = accumulatedInterest;

            this.DealerOrderVersion = dealerOrderVersion;
            this.DealerOrderVersionLinkId = dealerOrderVersionLinkId;
            this.DealerOrderGroupId = dealerOrderGroupId;

            this.LimitPrice = limitPrice;
            this.AverageFillPrice = averageFillPrice;
            this.OrderedVolume = orderedVolume;
            this.FilledVolume = filledVolume;
            this.OptionStrikePrice = optionStrikePrice;
            this.OptionExpirationDate = optionExpirationDate;
            this.OptionEuropeanAmerican = tradeOptionEuropeanAmerican;
        }

        public decimal? AccumulatedInterest { get; }

        public Money? AverageFillPrice { get; }

        public OrderCleanDirty CleanDirty { get; }

        public DateTime? CreatedDate { get; }

        public Currency Currency { get; }

        public string DealerCounterParty { get; }

        public string DealerId { get; }

        public string DealerName { get; }

        public string DealerOrderGroupId { get; }

        public string DealerOrderId { get; } // the client id for the trade

        public string DealerOrderVersion { get; }

        public string DealerOrderVersionLinkId { get; }

        public decimal? FilledVolume { get; }

        public IFinancialInstrument Instrument { get; }

        public Money? LimitPrice { get; }

        public string Notes { get; }

        public OptionEuropeanAmerican OptionEuropeanAmerican { get; }

        public DateTime? OptionExpirationDate { get; }

        public decimal? OptionStrikePrice { get; }

        public OrderDirections OrderDirection { get; }

        public decimal? OrderedVolume { get; }

        public OrderTypes OrderType { get; }

        public Order ParentOrder { get; set; }

        public string ReddeerDealerOrderId { get; } // primary key

        public Currency SettlementCurrency { get; }

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
    }
}