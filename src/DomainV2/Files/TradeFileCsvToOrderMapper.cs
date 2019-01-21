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
            
            if (order.DealerOrders == null
                || !order.DealerOrders.Any())
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

            foreach (var trad in order.DealerOrders)
            {
                var csv = new TradeFileCsv();
                csv = MapTradeFields2(trad, csv);
                csv = MapOrderFields(order, csv);
                csvs.Add(csv);
            }

            return csvs.ToArray();
        }

        private TradeFileCsv MapTradeFields2(DealerOrder trad, TradeFileCsv csv)
        {
            csv.DealerOrderId = trad.DealerOrderId;
            csv.DealerOrderPlacedDate = trad.PlacedDate?.ToString();
            csv.DealerOrderBookedDate = trad.BookedDate?.ToString();
            csv.DealerOrderAmendedDate = trad.AmendedDate?.ToString();
            csv.DealerOrderRejectedDate = trad.RejectedDate?.ToString();
            csv.DealerOrderCancelledDate = trad.CancelledDate?.ToString();
            csv.DealerOrderFilledDate = trad.FilledDate?.ToString();
            csv.DealerOrderDealerId = trad.DealerId;
            csv.DealerOrderCounterParty = trad.DealerCounterParty;
            csv.DealerOrderType = ((int?)trad.OrderType).ToString();
            csv.DealerOrderDirection = ((int?)trad.OrderDirection).ToString();
            csv.DealerOrderCurrency = trad.Currency.Value;
            csv.DealerOrderLimitPrice = trad.LimitPrice?.Value.ToString();
            csv.DealerOrderAverageFillPrice = trad.AverageFillPrice?.Value.ToString();
            csv.DealerOrderOrderedVolume = trad.OrderedVolume?.ToString();
            csv.DealerOrderFilledVolume = trad.FilledVolume?.ToString();
            csv.DealerOrderOptionStrikePrice = trad.OptionStrikePrice?.ToString();
            csv.DealerOrderOptionExpirationDate = trad.OptionStrikePrice?.ToString();
            csv.DealerOrderOptionEuropeanAmerican = ((int?)trad.OptionEuropeanAmerican).ToString();

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
            csv.OrderPlacedDate = order.PlacedDate.ToString();
            csv.OrderBookedDate = order.BookedDate.ToString();
            csv.OrderAmendedDate = order.AmendedDate.ToString();
            csv.OrderRejectedDate = order.RejectedDate.ToString();
            csv.OrderCancelledDate = order.CancelledDate.ToString();
            csv.OrderFilledDate = order.FilledDate.ToString();
            csv.OrderType = ((int?) order.OrderType).ToString();
            csv.OrderDirection = ((int?) order.OrderDirection).ToString();
            csv.OrderCurrency = order.OrderCurrency.Value;
            csv.OrderLimitPrice = order.OrderLimitPrice?.Value.ToString();
            csv.OrderAverageFillPrice = order.OrderAverageFillPrice?.Value.ToString();
            csv.OrderOrderedVolume = order.OrderOrderedVolume?.ToString();
            csv.OrderFilledVolume = order.OrderFilledVolume?.ToString();
            csv.OrderTraderId = order.OrderTraderId;
            csv.OrderTraderName = order.OrderTraderName;
            csv.OrderClearingAgent = order.OrderClearingAgent;
            csv.OrderDealingInstructions = order.OrderDealingInstructions;

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
            
            var trade = MapTrade(csv);
            var trades = trade != null ? new[] {trade} : null;
            var order = MapOrder(csv, trades);

            return order;
        }

        public int FailedParseTotal { get; set; }

        private Order MapOrder(TradeFileCsv csv, IReadOnlyCollection<DealerOrder> dealerOrder)
        {
            if (dealerOrder == null)
            {
                dealerOrder = new DealerOrder[0];
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
            var orderDirection = MapToEnum<OrderDirections>(csv.OrderDirection);
            var orderCurrency = new Currency(csv.OrderCurrency);
            var orderSettlementCurrency =
                !string.IsNullOrWhiteSpace(csv.OrderSettlementCurrency) 
                    ? (Currency?)new Currency(csv.OrderSettlementCurrency) 
                    : null;

            var orderCleanDirty = MapToEnum<OrderCleanDirty>(csv.OrderCleanDirty);

            var limitPrice = new CurrencyAmount(MapDecimal(csv.OrderLimitPrice), csv.OrderCurrency);
            var averagePrice = new CurrencyAmount(MapDecimal(csv.OrderAverageFillPrice), csv.OrderCurrency);

            var orderOptionStrikePrice = new CurrencyAmount(MapDecimal(csv.OrderOptionStrikePrice), csv.OrderCurrency);
            var orderOptionExpirationDate = MapDate(csv.OrderOptionExpirationDate);
            var orderOptionEuropeanAmerican = MapToEnum<OptionEuropeanAmerican>(csv.OrderOptionEuropeanAmerican);

            var orderedVolume = MapLong(csv.OrderOrderedVolume);
            var filledVolume = MapLong(csv.OrderFilledVolume);

            var accInterest = MapDecimal(csv.OrderAccumulatedInterest);
            
            return new Order(
                instrument,
                market,
                null,
                csv.OrderId,
                DateTime.UtcNow,
                csv.OrderVersion,
                csv.OrderVersionLinkId,
                csv.OrderGroupId,
                placedDate,
                bookedDate,
                amendedDate,
                rejectedDate,
                cancelledDate,
                filledDate,
                orderType,
                orderDirection,
                orderCurrency,
                orderSettlementCurrency,
                orderCleanDirty,
                accInterest,
                limitPrice,
                averagePrice,
                orderedVolume,
                filledVolume,
                csv.OrderTraderId,
                csv.OrderTraderName,
                csv.OrderClearingAgent,
                csv.OrderDealingInstructions,
                orderOptionStrikePrice,
                orderOptionExpirationDate,
                orderOptionEuropeanAmerican,
                dealerOrder);
        }

        private DealerOrder MapTrade(TradeFileCsv csv)
        {
            if (string.IsNullOrWhiteSpace(csv.DealerOrderId))
                return null;

            var instrument = MapInstrument(csv);

            var placedDate = MapDate(csv.DealerOrderPlacedDate);
            var bookedDate = MapDate(csv.DealerOrderBookedDate);
            var amendedDate = MapDate(csv.DealerOrderAmendedDate);
            var rejectedDate = MapDate(csv.DealerOrderRejectedDate);
            var cancelledDate = MapDate(csv.DealerOrderCancelledDate);
            var filledDate = MapDate(csv.DealerOrderFilledDate);

            var dealerOrderType = MapToEnum<OrderTypes>(csv.DealerOrderType);
            var dealerOrderDirection = MapToEnum<OrderDirections>(csv.DealerOrderDirection);
            var dealerOrderCurrency = new Currency(csv.DealerOrderCurrency);
            var dealerOrderSettlementCurrency = new Currency(csv.DealerOrderSettlementCurrency);

            var limitPrice = new CurrencyAmount(MapDecimal(csv.DealerOrderLimitPrice), csv.DealerOrderCurrency);
            var averagePrice = new CurrencyAmount(MapDecimal(csv.DealerOrderAverageFillPrice), csv.DealerOrderCurrency);
            var cleanDirty = MapToEnum<OrderCleanDirty>(csv.DealerOrderCleanDirty);
            var euroAmerican = MapToEnum<OptionEuropeanAmerican>(csv.DealerOrderOptionEuropeanAmerican);

            var orderedVolume = MapLong(csv.DealerOrderOrderedVolume);
            var filledVolume = MapLong(csv.DealerOrderFilledVolume);

            var accumulatedInterest = MapDecimal(csv.DealerOrderAccumulatedInterest);
            var optionStrikePrice = MapDecimal(csv.DealerOrderOptionStrikePrice);
            var optionExpirationDate = MapDate(csv.DealerOrderOptionExpirationDate);

            return new DealerOrder(
                instrument,
                string.Empty,
                csv.DealerOrderId,
                placedDate,
                bookedDate,
                amendedDate,
                rejectedDate,
                cancelledDate,
                filledDate,
                DateTime.UtcNow,
                csv.DealerOrderDealerId,
                csv.DealerOrderDealerName,
                csv.DealerOrderNotes,
                csv.DealerOrderCounterParty,
                dealerOrderType,
                dealerOrderDirection,
                dealerOrderCurrency,
                dealerOrderSettlementCurrency,
                cleanDirty,
                accumulatedInterest,
                csv.DealerOrderVersion,
                csv.DealerOrderVersionLinkId,
                csv.DealerOrderGroupId,
                limitPrice,
                averagePrice,
                orderedVolume,
                filledVolume,
                optionStrikePrice,
                optionExpirationDate,
                euroAmerican);
        }

        private T MapToEnum<T>(string propertyValue) where T : struct, IConvertible
        {
            propertyValue = propertyValue?.ToUpper() ?? string.Empty;

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
                    null,
                    csv.InstrumentClientIdentifier,
                    csv.InstrumentSedol,
                    csv.InstrumentIsin,
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
                csv.OrderCurrency ?? csv.DealerOrderCurrency ?? string.Empty,
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
