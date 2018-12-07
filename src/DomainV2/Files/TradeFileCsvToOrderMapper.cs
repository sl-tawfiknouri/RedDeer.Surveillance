using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Files.Interfaces;
using DomainV2.Financial;
using DomainV2.Trading;

namespace DomainV2.Files
{
    public class TradeFileCsvToOrderMapper : ITradeFileCsvToOrderMapper
    {
        /// <summary>
        /// Assumption => csv is validated
        /// </summary>
        public Order Map(TradeFileCsv csv)
        {
            if (csv == null)
            {
                FailedParseTotal += 1;
                return null;
            }
            
            var transaction = MapTransaction(csv);
            var trade = MapTrade(csv, new [] {transaction});
            var order = MapOrder(csv, new [] {trade});

            return order;
        }

        public int FailedParseTotal { get; set; }

        private Order MapOrder(TradeFileCsv csv, IReadOnlyCollection<Trade> trades)
        {
            if (trades == null)
            {
                trades = new Trade[0];
            }

            var instrument = MapInstrument(csv);
            var market = MapMarket(csv);

            var placedDate = MapDate(csv.OrderPlacedDate);
            var bookedDate = MapDate(csv.OrderBookedDate);
            var amendedDate = MapDate(csv.OrderAmendedDate);
            var rejectedDate = MapDate(csv.OrderRejectedDate);
            var cancelledDate = MapDate(csv.OrderCancelledDate);
            var filledDate = MapDate(csv.OrderFilledDate);

            var orderType = MapToEnum<OrderTypes>(csv.OrderType);
            var orderPosition = MapToEnum<OrderPositions>(csv.OrderPosition);
            var orderCurrency = new Currency(csv.OrderCurrency);
            var limitPrice = new CurrencyAmount(MapDecimal(csv.OrderLimitPrice), csv.OrderCurrency);
            var averagePrice = new CurrencyAmount(MapDecimal(csv.OrderAveragePrice), csv.OrderCurrency);

            var orderedVolume = MapLong(csv.OrderOrderedVolume);
            var filledVolume = MapLong(csv.OrderFilledVolume);
            
            return new Order(
                instrument,
                market,
                null,
                csv.OrderId,
                placedDate,
                bookedDate,
                amendedDate,
                rejectedDate,
                cancelledDate,
                filledDate,
                orderType,
                orderPosition,
                orderCurrency,
                limitPrice,
                averagePrice,
                orderedVolume,
                filledVolume,
                csv.OrderPortfolioManager,
                csv.OrderTraderId,
                csv.OrderExecutingBroker,
                csv.OrderClearingAgent,
                csv.OrderDealingInstructions,
                csv.OrderStrategy,
                csv.OrderRationale,
                csv.OrderFund,
                csv.OrderClientAccountAttributionId,
                trades);
        }

        private Trade MapTrade(TradeFileCsv csv, IReadOnlyCollection<Transaction> transactions)
        {
            if (transactions == null)
            {
                transactions = new Transaction[0];
            }

            var instrument = MapInstrument(csv);

            var placedDate = MapDate(csv.TradePlacedDate);
            var bookedDate = MapDate(csv.TradeBookedDate);
            var amendedDate = MapDate(csv.TradeAmendedDate);
            var rejectedDate = MapDate(csv.TradeRejectedDate);
            var cancelledDate = MapDate(csv.TradeCancelledDate);
            var filledDate = MapDate(csv.TradeFilledDate);

            var tradeType = MapToEnum<OrderTypes>(csv.TradeType);
            var tradePosition = MapToEnum<OrderPositions>(csv.TradePosition);
            var tradeCurrency = new Currency(csv.TradeCurrency);
            var limitPrice = new CurrencyAmount(MapDecimal(csv.TradeLimitPrice), csv.TradeCurrency);
            var averagePrice = new CurrencyAmount(MapDecimal(csv.TradeAveragePrice), csv.TradeCurrency);

            var orderedVolume = MapLong(csv.TradeOrderedVolume);
            var filledVolume = MapLong(csv.TradeFilledVolume);

            var optionStrikePrice = MapDecimal(csv.TradeOptionStrikePrice);
            var expirationDate = MapDate(csv.TradeOptionExpirationDate);

            return new Trade(
                instrument,
                string.Empty,
                csv.TradeId,
                placedDate,
                bookedDate,
                amendedDate,
                rejectedDate,
                cancelledDate,
                filledDate,
                csv.TraderId,
                csv.TradeCounterParty,
                tradeType,
                tradePosition,
                tradeCurrency,
                limitPrice,
                averagePrice,
                orderedVolume,
                filledVolume,
                optionStrikePrice,
                expirationDate,
                csv.TradeOptionEuropeanAmerican,
                transactions);
        }

        private Transaction MapTransaction(TradeFileCsv csv)
        {
            var instrument = MapInstrument(csv);

            var placedDate = MapDate(csv.TransactionPlacedDate);
            var bookedDate = MapDate(csv.TransactionBookedDate);
            var amendedDate = MapDate(csv.TransactionAmendedDate);
            var rejectedDate = MapDate(csv.TransactionRejectedDate);
            var cancelledDate = MapDate(csv.TransactionCancelledDate);
            var filledDate = MapDate(csv.TransactionFilledDate);

            var tradeType = MapToEnum<OrderTypes>(csv.TransactionPosition);
            var tradePosition = MapToEnum<OrderPositions>(csv.TransactionPosition);
            var tradeCurrency = new Currency(csv.TransactionCurrency);
            var limitPrice = new CurrencyAmount(MapDecimal(csv.TransactionLimitPrice), csv.TransactionCurrency);
            var averagePrice = new CurrencyAmount(MapDecimal(csv.TransactionAveragePrice), csv.TransactionCurrency);

            var orderedVolume = MapLong(csv.TransactionOrderedVolume);
            var filledVolume = MapLong(csv.TransactionFilledVolume);

            return new Transaction(
                instrument,
                string.Empty,
                csv.TransactionId,
                placedDate,
                bookedDate,
                amendedDate,
                rejectedDate,
                cancelledDate,
                filledDate,
                csv.TransactionTraderId,
                csv.TransactionCounterParty,
                tradeType,
                tradePosition,
                tradeCurrency,
                limitPrice,
                averagePrice,
                orderedVolume,
                filledVolume);
        }

        private T MapToEnum<T>(string propertyValue) where T : struct, IConvertible
        {
            Enum.TryParse(propertyValue, out T result);

            return result;
        }

        private DateTime? MapDate(string date)
        {
            if (string.IsNullOrWhiteSpace(date))
            {
                return null;
            }

            var success = DateTime.TryParse(date, out var result);

            if (success)
                return result;

            return null;
        }

        private decimal? MapDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var success = decimal.TryParse(value, out var result);

            if (success)
                return result;

            return null;
        }

        private long? MapLong(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var success = long.TryParse(value, out var result);

            if (success)
                return result;

            return null;
        }

        private FinancialInstrument MapInstrument(TradeFileCsv csv)
        {
            var identifiers =
                new InstrumentIdentifiers(
                    string.Empty,
                    string.Empty,
                    csv.InstrumentClientIdentifier,
                    csv.InstrumentSedol,
                    csv.InstrumentSedol,
                    csv.InstrumentFigi,
                    csv.InstrumentCusip,
                    csv.InstrumentExchangeSymbol,
                    csv.InstrumentLei,
                    csv.InstrumentBloombergTicker,
                    csv.InstrumentUnderlyingSedol,
                    csv.InstrumentUnderlyingIsin,
                    csv.InstrumentUnderlyingFigi,
                    csv.InstrumentUnderlyingCusip,
                    csv.InstrumentUnderlyingLei,
                    csv.InstrumentUnderlyingExchangeSymbol,
                    csv.InstrumentUnderlyingBloombergTicker);

            return new FinancialInstrument(
                MapCfi(csv.InstrumentCfi),
                identifiers,
                csv.InstrumentName ?? string.Empty,
                csv.InstrumentCfi ?? string.Empty,
                csv.InstrumentIssuerIdentifier ?? string.Empty,
                csv.InstrumentUnderlyingName ?? string.Empty,
                csv.InstrumentUnderlyingCfi ?? string.Empty,
                csv.InstrumentUnderlyingIssuerIdentifier ?? string.Empty);
        }

        private Market MapMarket(TradeFileCsv csv)
        {
            Enum.TryParse(csv.MarketType, out MarketTypes marketType);

            return new Market(string.Empty, csv.MarketIdentifierCode, csv.MarketName, marketType);
        }

        private InstrumentTypes MapCfi(string cfi)
        {
            if (string.IsNullOrWhiteSpace(cfi))
            {
                return InstrumentTypes.None;
            }

            cfi = cfi.ToLower();

            if (cfi.Take(1).ToString() == "e")
            {
                return InstrumentTypes.Equity;
            }

            if (cfi.Take(2).ToString() == "db")
            {
                return InstrumentTypes.Bond;
            }

            if (cfi.Take(2).ToString() == "oc")
            {
                return InstrumentTypes.OptionCall;
            }

            if (cfi.Take(2).ToString() == "op")
            {
                return InstrumentTypes.OptionPut;
            }

            return InstrumentTypes.Unknown;
        }
    }
}
