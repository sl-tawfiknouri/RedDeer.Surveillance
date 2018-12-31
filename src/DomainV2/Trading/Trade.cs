﻿using System;
using System.Collections.Generic;
using DomainV2.Financial;
using DomainV2.Financial.Interfaces;

namespace DomainV2.Trading
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
            OrderTypes tradeType,
            OrderPositions tradePosition,
            Currency tradeCurrency, 
            CurrencyAmount? tradeLimitPrice,
            CurrencyAmount? tradeAveragePrice,
            long? tradeOrderedVolume,
            long? tradeFilledVolume,
            decimal? tradeOptionStrikePrice,
            DateTime? tradeOptionExpirationDate,
            string tradeOptionEuropeanAmerican,
            IReadOnlyCollection<Transaction> transaction)
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
            TradeType = tradeType;
            TradePosition = tradePosition;
            TradeCurrency = tradeCurrency;
            TradeLimitPrice = tradeLimitPrice;
            TradeAveragePrice = tradeAveragePrice;
            TradeOrderedVolume = tradeOrderedVolume;
            TradeFilledVolume = tradeFilledVolume;
            TradeOptionStrikePrice = tradeOptionStrikePrice;
            TradeOptionExpirationDate = tradeOptionExpirationDate;
            TradeOptionEuropeanAmerican = tradeOptionEuropeanAmerican ?? string.Empty;
            Transactions = transaction ?? new Transaction[0];
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
        public OrderTypes TradeType { get; }
        public OrderPositions TradePosition { get; }
        public Currency TradeCurrency { get; }
        public CurrencyAmount? TradeLimitPrice { get; }
        public CurrencyAmount? TradeAveragePrice { get; }
        public long? TradeOrderedVolume { get; }
        public long? TradeFilledVolume { get; }
        public decimal? TradeOptionStrikePrice { get; }
        public DateTime? TradeOptionExpirationDate { get; }
        public string TradeOptionEuropeanAmerican { get; }
        public IReadOnlyCollection<Transaction> Transactions { get; set; }
        public Order ParentOrder { get; set; }
    }
}