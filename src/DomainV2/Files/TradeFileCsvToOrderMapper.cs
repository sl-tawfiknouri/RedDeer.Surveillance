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
        public TradeFileCsv[] Map(Order order)
        {
            if (order == null)
            {
                return null;
            }

            var csv = new TradeFileCsv();
            csv = MapOrderFields(order, csv);
            
            if (order.Trades == null
                || !order.Trades.Any())
            {
                return new TradeFileCsv[] { csv };
            }

            var result = MapTradeFields(order);

            return result;
        }

        private TradeFileCsv[] MapTradeFields(Order order)
        {
            // ReSharper disable once IdentifierTypo
            var csvs = new List<TradeFileCsv>();

            foreach (var trad in order.Trades)
            {
                if (trad.Transactions == null
                    || !trad.Transactions.Any())
                {
                    var csv = new TradeFileCsv();
                    csv = MapTradeFields2(trad, csv);
                    csv = MapOrderFields(order, csv);
                    csvs.Add(csv);
                }
                else
                {
                    foreach (var tran in trad.Transactions)
                    {
                        var csv = new TradeFileCsv();
                        csv = MapTransactionFields(tran, csv);
                        csv = MapTradeFields2(trad, csv);
                        csv = MapOrderFields(order, csv);
                        csvs.Add(csv);
                    }
                }
            }

            return csvs.ToArray();
        }

        private TradeFileCsv MapTransactionFields(Transaction transaction, TradeFileCsv csv)
        {
            csv.TransactionId = transaction.TransactionId;
            csv.TransactionPlacedDate = transaction.TransactionPlacedDate?.ToString();
            csv.TransactionBookedDate = transaction.TransactionBookedDate?.ToString();
            csv.TransactionAmendedDate = transaction.TransactionAmendedDate?.ToString();
            csv.TransactionRejectedDate = transaction.TransactionRejectedDate?.ToString();
            csv.TransactionCancelledDate = transaction.TransactionCancelledDate?.ToString();
            csv.TransactionFilledDate = transaction.TransactionFilledDate?.ToString();
            csv.TransactionTraderId = transaction.TransactionTraderId;
            csv.TransactionCounterParty = transaction.TransactionCounterParty;
            csv.TransactionType = ((int?)transaction.TransactionType)?.ToString();
            csv.TransactionPosition = ((int?) transaction.TransactionPosition)?.ToString();
            csv.TransactionCurrency = transaction?.TransactionCurrency.Value;
            csv.TransactionLimitPrice = transaction?.TransactionLimitPrice?.Value.ToString();
            csv.TransactionAveragePrice = transaction?.TransactionAveragePrice.Value.ToString();
            csv.TransactionOrderedVolume = transaction?.TransactionOrderedVolume?.ToString();
            csv.TransactionFilledVolume = transaction?.TransactionFilledVolume?.ToString();

            return csv;
        }

        private TradeFileCsv MapTradeFields2(Trade trad, TradeFileCsv csv)
        {
            csv.TradeId = trad.TradeId;
            csv.TradePlacedDate = trad.TradePlacedDate?.ToString();
            csv.TradeBookedDate = trad.TradeBookedDate?.ToString();
            csv.TradeAmendedDate = trad.TradeAmendedDate?.ToString();
            csv.TradeRejectedDate = trad.TradeRejectedDate?.ToString();
            csv.TradeCancelledDate = trad.TradeCancelledDate?.ToString();
            csv.TradeFilledDate = trad.TradeFilledDate?.ToString();
            csv.TraderId = trad.TraderId;
            csv.TradeCounterParty = trad.TradeCounterParty;
            csv.TradeType = ((int?)trad.TradeType).ToString();
            csv.TradePosition = ((int?)trad.TradePosition).ToString();
            csv.TradeCurrency = trad.TradeCurrency.Value;
            csv.TradeLimitPrice = trad.TradeLimitPrice?.Value.ToString();
            csv.TradeAveragePrice = trad.TradeAveragePrice?.Value.ToString();
            csv.TradeOrderedVolume = trad.TradeOrderedVolume?.ToString();
            csv.TradeFilledVolume = trad.TradeFilledVolume?.ToString();
            csv.TradeOptionStrikePrice = trad.TradeOptionStrikePrice?.ToString();
            csv.TradeOptionExpirationDate = trad.TradeOptionStrikePrice?.ToString();
            csv.TradeOptionEuropeanAmerican = trad.TradeOptionEuropeanAmerican;

            return csv;
        }

        private TradeFileCsv MapOrderFields(Order order, TradeFileCsv csv)
        {
            csv.MarketType = ((int?) order.Market.Type)?.ToString();
            csv.MarketIdentifierCode = order.Market.MarketIdentifierCode;
            csv.MarketName = order.Market.Name;
            csv.InstrumentName = order.Instrument.Name;
            csv.InstrumentCfi = order.Instrument.Cfi;
            csv.InstrumentIssuerIdentifier = order.Instrument.IssuerIdentifier;
            csv.InstrumentClientIdentifier = order.Instrument.Identifiers.ClientIdentifier;
            csv.InstrumentSedol = order.Instrument.Identifiers.Sedol;
            csv.InstrumentIsin = order.Instrument.Identifiers.Isin;
            csv.InstrumentFigi = order.Instrument.Identifiers.Figi;
            csv.InstrumentCusip = order.Instrument.Identifiers.Cusip;
            csv.InstrumentLei = order.Instrument.Identifiers.Lei;
            csv.InstrumentExchangeSymbol = order.Instrument.Identifiers.ExchangeSymbol;
            csv.InstrumentBloombergTicker = order.Instrument.Identifiers.BloombergTicker;

            csv.InstrumentUnderlyingName = order.Instrument.UnderlyingName;
            csv.InstrumentUnderlyingCfi = order.Instrument.UnderlyingCfi;
            csv.InstrumentUnderlyingIssuerIdentifier = order.Instrument.UnderlyingIssuerIdentifier;
            csv.InstrumentUnderlyingClientIdentifier = order.Instrument.Identifiers.UnderlyingClientIdentifier;
            csv.InstrumentUnderlyingSedol = order.Instrument.Identifiers.UnderlyingSedol;
            csv.InstrumentUnderlyingIsin = order.Instrument.Identifiers.UnderlyingIsin;
            csv.InstrumentUnderlyingFigi = order.Instrument.Identifiers.UnderlyingFigi;
            csv.InstrumentUnderlyingCusip = order.Instrument.Identifiers.UnderlyingCusip;
            csv.InstrumentUnderlyingLei = order.Instrument.Identifiers.UnderlyingLei;
            csv.InstrumentUnderlyingExchangeSymbol = order.Instrument.Identifiers.UnderlyingExchangeSymbol;
            csv.InstrumentUnderlyingBloombergTicker = order.Instrument.Identifiers.UnderlyingBloombergTicker;

            csv.OrderId = order.OrderId;
            csv.OrderPlacedDate = order.OrderPlacedDate.ToString();
            csv.OrderBookedDate = order.OrderBookedDate.ToString();
            csv.OrderAmendedDate = order.OrderAmendedDate.ToString();
            csv.OrderRejectedDate = order.OrderRejectedDate.ToString();
            csv.OrderCancelledDate = order.OrderCancelledDate.ToString();
            csv.OrderFilledDate = order.OrderFilledDate.ToString();
            csv.OrderType = ((int?) order.OrderType).ToString();
            csv.OrderPosition = ((int?) order.OrderPosition).ToString();
            csv.OrderCurrency = order.OrderCurrency.Value;
            csv.OrderLimitPrice = order.OrderLimitPrice?.Value.ToString();
            csv.OrderAveragePrice = order.OrderAveragePrice?.Value.ToString();
            csv.OrderOrderedVolume = order.OrderOrderedVolume?.ToString();
            csv.OrderFilledVolume = order.OrderFilledVolume?.ToString();
            csv.OrderPortfolioManager = order.OrderPortfolioManager;
            csv.OrderTraderId = order.OrderTraderId;
            csv.OrderExecutingBroker = order.OrderExecutingBroker;
            csv.OrderClearingAgent = order.OrderClearingAgent;
            csv.OrderDealingInstructions = order.OrderDealingInstructions;
            csv.OrderStrategy = order.OrderStrategy;
            csv.OrderRationale = order.OrderRationale;
            csv.OrderFund = order.OrderFund;
            csv.OrderClientAccountAttributionId = order.OrderClientAccountAttributionId;

            return csv;
        }

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
            var transactions = transaction != null ? new[] {transaction} : null;
            var trade = MapTrade(csv, transactions);
            var trades = trade != null ? new[] {trade} : null;
            var order = MapOrder(csv, trades);

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
            if (string.IsNullOrWhiteSpace(csv.TradeId))
                return null;

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
            if (string.IsNullOrWhiteSpace(csv.TransactionId))
                return null;

            var instrument = MapInstrument(csv);

            var placedDate = MapDate(csv.TransactionPlacedDate);
            var bookedDate = MapDate(csv.TransactionBookedDate);
            var amendedDate = MapDate(csv.TransactionAmendedDate);
            var rejectedDate = MapDate(csv.TransactionRejectedDate);
            var cancelledDate = MapDate(csv.TransactionCancelledDate);
            var filledDate = MapDate(csv.TransactionFilledDate);

            var tradeType = MapToEnum<OrderTypes>(csv.TransactionType);
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
                    csv.InstrumentUnderlyingBloombergTicker,
                    csv.InstrumentUnderlyingClientIdentifier);

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
