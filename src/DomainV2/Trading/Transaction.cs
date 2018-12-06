using System;
using DomainV2.Financial;
using DomainV2.Financial.Interfaces;

namespace DomainV2.Trading
{
    public class Transaction
    {
        public Transaction(
            IFinancialInstrument instrument,
            string reddeerTransactionId,
            string transactionId,
            DateTime? transactionPlacedDate,
            DateTime? transactionBookedDate,
            DateTime? transactionAmendedDate,
            DateTime? transactionRejectedDate,
            DateTime? transactionCancelledDate,
            DateTime? transactionFilledDate,
            string transactionTraderId,
            string transactionCounterParty,
            OrderTypes transactionType,
            OrderPositions transactionPosition,
            Currency transactionCurrency,
            CurrencyAmount? transactionLimitPrice,
            CurrencyAmount? transactionAveragePrice,
            long? transactionOrderedVolume,
            long? transactionFilledVolume)
        {
            Instrument = instrument;
            ReddeerTransactionId = reddeerTransactionId ?? string.Empty; ;
            TransactionId = transactionId ?? string.Empty;
            TransactionPlacedDate = transactionPlacedDate;
            TransactionBookedDate = transactionBookedDate;
            TransactionAmendedDate = transactionAmendedDate;
            TransactionRejectedDate = transactionRejectedDate;
            TransactionCancelledDate = transactionCancelledDate;
            TransactionFilledDate = transactionFilledDate;
            TransactionTraderId = transactionTraderId ?? string.Empty; ;
            TransactionCounterParty = transactionCounterParty ?? string.Empty; ;
            TransactionType = transactionType;
            TransactionPosition = transactionPosition;
            TransactionCurrency = transactionCurrency;
            TransactionLimitPrice = transactionLimitPrice;
            TransactionAveragePrice = transactionAveragePrice;
            TransactionOrderedVolume = transactionOrderedVolume;
            TransactionFilledVolume = transactionFilledVolume;
        }

        public IFinancialInstrument Instrument { get; }
        public string ReddeerTransactionId { get; } // primary key
        public string TransactionId { get; } // the client id for the transaction
        public DateTime? TransactionPlacedDate { get; }
        public DateTime? TransactionBookedDate { get; }
        public DateTime? TransactionAmendedDate { get; }
        public DateTime? TransactionRejectedDate { get; }
        public DateTime? TransactionCancelledDate { get; }
        public DateTime? TransactionFilledDate { get; }
        public string TransactionTraderId { get; }
        public string TransactionCounterParty { get; }
        public OrderTypes TransactionType { get; }
        public OrderPositions TransactionPosition { get; }
        public Currency TransactionCurrency { get; }
        public CurrencyAmount? TransactionLimitPrice { get; }
        public CurrencyAmount? TransactionAveragePrice { get; }
        public long? TransactionOrderedVolume { get; }
        public long? TransactionFilledVolume { get; }
        public Trade ParentTrade { get; set; } // parent trade the transaction is a part of can be null
    }
}
