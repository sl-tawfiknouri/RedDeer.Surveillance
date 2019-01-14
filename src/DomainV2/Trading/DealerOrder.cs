using System;
using System.Linq;
using DomainV2.Financial;
using DomainV2.Financial.Interfaces;

namespace DomainV2.Trading
{
    public class DealerOrder
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
            string traderId,
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
            CurrencyAmount? limitPrice,
            CurrencyAmount? averageFillPrice,
            long? orderedVolume,
            long? filledVolume,
            decimal? optionStrikePrice,
            DateTime? optionExpirationDate,
            OptionEuropeanAmerican tradeOptionEuropeanAmerican)
        {
            Instrument = instrument ?? throw new ArgumentNullException(nameof(instrument));
            ReddeerDealerOrderId = reddeerTradeId ?? string.Empty;
            DealerOrderId = tradeId ?? string.Empty;

            PlacedDate = placedDate;
            BookedDate = bookedDate;
            AmendedDate = amendedDate;
            RejectedDate = rejectedDate;
            CancelledDate = cancelledDate;
            FilledDate = filledDate;

            DealerId = traderId ?? string.Empty;
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

        public DateTime? PlacedDate { get; }
        public DateTime? BookedDate { get; }
        public DateTime? AmendedDate { get; }
        public DateTime? RejectedDate { get; }
        public DateTime? CancelledDate { get; }
        public DateTime? FilledDate { get; }

        public string DealerId { get; }
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
        
        public CurrencyAmount? LimitPrice { get; }
        public CurrencyAmount? AverageFillPrice { get; }
        public long? OrderedVolume { get; }
        public long? FilledVolume { get; }

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
                return DateTime.Now;
            }

            // placed should never be null i.e. this shouldn't call datetime.now
            return filteredDates.OrderByDescending(fd => fd).FirstOrDefault() ?? DateTime.Now;
        }
    }
}
