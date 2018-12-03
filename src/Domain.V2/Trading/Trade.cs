using System;
using Domain.V2.Financial.Interfaces;

namespace Domain.V2.Trading
{
    public class Trade
    {
        public Trade(
            IFinancialInstrument instrument,
            string reddeerTradeId,
            string tradeId,
            DateTime? tradePlacedDate,
            DateTime? tradeBookedDate,
            DateTime? tradeAmendedDate,
            DateTime? tradeRejectedDate,
            DateTime? tradeCancelledDate,
            DateTime? tradeFilledDate,
            string traderId,
            string tradeCounterParty,
            string tradeType,
            string tradePosition,
            string tradeCurrency, 
            decimal? tradeLimitPrice,
            decimal? tradeAveragePrice,
            long? tradeOrderedVolume,
            long? tradeFilledVolume,
            decimal? tradeOptionStrikePrice,
            DateTime? tradeOptionExpirationDate,
            string tradeOptionEuropeanAmerican)
        {
            Instrument = instrument ?? throw new ArgumentNullException(nameof(instrument));
            ReddeerTradeId = reddeerTradeId ?? string.Empty;
            TradeId = tradeId ?? string.Empty;
            TradePlacedDate = tradePlacedDate;
            TradeBookedDate = tradeBookedDate;
            TradeAmendedDate = tradeAmendedDate;
            TradeRejectedDate = tradeRejectedDate;
            TradeCancelledDate = tradeCancelledDate;
            TradeFilledDate = tradeFilledDate;
            TraderId = traderId ?? string.Empty;
            TradeCounterParty = tradeCounterParty ?? string.Empty;
            TradeType = tradeType ?? string.Empty;
            TradePosition = tradePosition ?? string.Empty;
            TradeCurrency = tradeCurrency ?? string.Empty;
            TradeLimitPrice = tradeLimitPrice;
            TradeAveragePrice = tradeAveragePrice;
            TradeOrderedVolume = tradeOrderedVolume;
            TradeFilledVolume = tradeFilledVolume;
            TradeOptionStrikePrice = tradeOptionStrikePrice;
            TradeOptionExpirationDate = tradeOptionExpirationDate;
            TradeOptionEuropeanAmerican = tradeOptionEuropeanAmerican ?? string.Empty;
        }

        public IFinancialInstrument Instrument { get; }
        public string ReddeerTradeId { get; } // primary key
        public string TradeId { get; } // the client id for the trade
        public DateTime? TradePlacedDate { get; }
        public DateTime? TradeBookedDate { get; }
        public DateTime? TradeAmendedDate { get; }
        public DateTime? TradeRejectedDate { get; }
        public DateTime? TradeCancelledDate { get; }
        public DateTime? TradeFilledDate { get; }
        public string TraderId { get; }
        public string TradeCounterParty { get; }
        public string TradeType { get; }
        public string TradePosition { get; }
        public string TradeCurrency { get; }
        public decimal? TradeLimitPrice { get; }
        public decimal? TradeAveragePrice { get; }
        public long? TradeOrderedVolume { get; }
        public long? TradeFilledVolume { get; }
        public decimal? TradeOptionStrikePrice { get; }
        public DateTime? TradeOptionExpirationDate { get; }
        public string TradeOptionEuropeanAmerican { get; }
    }
}
