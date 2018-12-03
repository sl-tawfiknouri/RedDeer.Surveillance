using System;
using Domain.V2.Financial.Interfaces;

namespace Domain.V2.Trading
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
            string transactionType,
            string transactionPosition,
            string transactionCurrency,
            decimal? transactionLimitPrice,
            decimal? transactionAveragePrice,
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
            TransactionType = transactionType ?? string.Empty; ;
            TransactionPosition = transactionPosition ?? string.Empty; ;
            TransactionCurrency = transactionCurrency ?? string.Empty; ;
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
        public string TransactionType { get; }
        public string TransactionPosition { get; }
        public string TransactionCurrency { get; }
        public decimal? TransactionLimitPrice { get; }
        public decimal? TransactionAveragePrice { get; }
        public long? TransactionOrderedVolume { get; }
        public long? TransactionFilledVolume { get; }
    }
}
