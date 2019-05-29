using System;
using System.Linq;
using Domain.Core.Financial.Assets;
using Domain.Core.Financial.Assets.Interfaces;
using Domain.Core.Financial.Money;

namespace Domain.Core.Trading.Orders
{
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
            : base(
                placedDate,
                bookedDate,
                amendedDate,
                rejectedDate,
                cancelledDate,
                filledDate)
        {
            Instrument = instrument ?? throw new ArgumentNullException(nameof(instrument));
            ReddeerDealerOrderId = reddeerTradeId ?? string.Empty;
            DealerOrderId = tradeId ?? string.Empty;

            CreatedDate = createdDate;

            DealerId = traderId ?? string.Empty;
            DealerName = dealerName ?? string.Empty;
            Notes = notes ?? string.Empty;
            DealerCounterParty = tradeCounterParty ?? string.Empty;
            OrderType = orderType;
            OrderDirection = orderDirection;
            Currency = currency;
            SettlementCurrency = settlementCurrency;
            CleanDirty = cleanDirty;
            AccumulatedInterest = accumulatedInterest;

            DealerOrderVersion = dealerOrderVersion;
            DealerOrderVersionLinkId = dealerOrderVersionLinkId;
            DealerOrderGroupId = dealerOrderGroupId;

            LimitPrice = limitPrice;
            AverageFillPrice = averageFillPrice;
            OrderedVolume = orderedVolume;
            FilledVolume = filledVolume;
            OptionStrikePrice = optionStrikePrice;
            OptionExpirationDate = optionExpirationDate;
            OptionEuropeanAmerican = tradeOptionEuropeanAmerican;
        }

        public IFinancialInstrument Instrument { get; }
        public string ReddeerDealerOrderId { get; } // primary key
        public string DealerOrderId { get; } // the client id for the trade

        public DateTime? CreatedDate { get; }

        public string DealerId { get; }
        public string DealerName { get; }
        public string Notes { get; }
        public string DealerCounterParty { get; }
        public OrderTypes OrderType { get; }
        public OrderDirections OrderDirection { get; }

        public Currency Currency { get; }
        public Currency SettlementCurrency { get; }
        public OrderCleanDirty CleanDirty { get; }
        public decimal? AccumulatedInterest { get; }

        public string DealerOrderVersion { get; }
        public string DealerOrderVersionLinkId { get; }
        public string DealerOrderGroupId { get; }
        
        public Money? LimitPrice { get; }
        public Money? AverageFillPrice { get; }
        public decimal? OrderedVolume { get; }
        public decimal? FilledVolume { get; }

        public decimal? OptionStrikePrice { get; }
        public DateTime? OptionExpirationDate { get; }
        public OptionEuropeanAmerican OptionEuropeanAmerican { get; }

        public Order ParentOrder { get; set; }

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
    }
}
