using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Domain.Core.Extensions;
using Domain.Core.Financial.Assets;
using Domain.Core.Financial.Money;
using Domain.Core.Markets;
using Domain.Core.Trading.Orders;
using SharedKernel.Files.Orders.Interfaces;

namespace SharedKernel.Files.Orders
{
    public class OrderFileToOrderSerialiser : IOrderFileToOrderSerialiser
    {
        public OrderFileContract[] Map(Order order)
        {
            if (order == null)
            {
                return null;
            }

            var csv = new OrderFileContract();
            csv = MapOrderFields(order, csv);
            
            if (order.DealerOrders == null
                || !order.DealerOrders.Any())
            {
                return new OrderFileContract[] { csv };
            }

            var result = MapTradeFields(order);

            return result;
        }

        private OrderFileContract[] MapTradeFields(Order order)
        {
            // ReSharper disable once IdentifierTypo
            var csvs = new List<OrderFileContract>();

            foreach (var trad in order.DealerOrders)
            {
                var csv = new OrderFileContract();
                csv = MapTradeFields2(trad, csv);
                csv = MapOrderFields(order, csv);
                csvs.Add(csv);
            }

            return csvs.ToArray();
        }

        private OrderFileContract MapTradeFields2(DealerOrder trad, OrderFileContract contract)
        {
            contract.DealerOrderId = trad.DealerOrderId;
            contract.DealerOrderPlacedDate = trad.PlacedDate?.ToString("yyyy-MM-ddTHH:mm:ss");
            contract.DealerOrderBookedDate = trad.BookedDate?.ToString("yyyy-MM-ddTHH:mm:ss");
            contract.DealerOrderAmendedDate = trad.AmendedDate?.ToString("yyyy-MM-ddTHH:mm:ss");
            contract.DealerOrderRejectedDate = trad.RejectedDate?.ToString("yyyy-MM-ddTHH:mm:ss");
            contract.DealerOrderCancelledDate = trad.CancelledDate?.ToString("yyyy-MM-ddTHH:mm:ss");
            contract.DealerOrderFilledDate = trad.FilledDate?.ToString("yyyy-MM-ddTHH:mm:ss");
            contract.DealerOrderDealerId = trad.DealerId;
            contract.DealerOrderCounterParty = trad.DealerCounterParty;
            contract.DealerOrderType = ((int?)trad.OrderType).ToString();
            contract.DealerOrderDirection = ((int?)trad.OrderDirection).ToString();
            contract.DealerOrderCurrency = trad.Currency.Code;
            contract.DealerOrderLimitPrice = trad.LimitPrice?.Value.ToString();
            contract.DealerOrderAverageFillPrice = trad.AverageFillPrice?.Value.ToString();
            contract.DealerOrderOrderedVolume = trad.OrderedVolume?.ToString();
            contract.DealerOrderFilledVolume = trad.FilledVolume?.ToString();
            contract.DealerOrderOptionStrikePrice = trad.OptionStrikePrice?.ToString();
            contract.DealerOrderOptionExpirationDate = trad.OptionStrikePrice?.ToString("yyyy-MM-ddTHH:mm:ss");
            contract.DealerOrderOptionEuropeanAmerican = ((int?)trad.OptionEuropeanAmerican).ToString();

            return contract;
        }

        private OrderFileContract MapOrderFields(Order order, OrderFileContract contract)
        {
            contract.MarketType = ((int?) order.Market.Type)?.ToString();
            contract.MarketIdentifierCode = order.Market.MarketIdentifierCode;
            contract.MarketName = order.Market.Name;
            contract.InstrumentName = order.Instrument.Name;
            contract.InstrumentCfi = order.Instrument.Cfi;
            contract.InstrumentIssuerIdentifier = order.Instrument.IssuerIdentifier;
            contract.InstrumentClientIdentifier = order.Instrument.Identifiers.ClientIdentifier;
            contract.InstrumentSedol = order.Instrument.Identifiers.Sedol;
            contract.InstrumentIsin = order.Instrument.Identifiers.Isin;
            contract.InstrumentFigi = order.Instrument.Identifiers.Figi;
            contract.InstrumentCusip = order.Instrument.Identifiers.Cusip;
            contract.InstrumentLei = order.Instrument.Identifiers.Lei;
            contract.InstrumentExchangeSymbol = order.Instrument.Identifiers.ExchangeSymbol;
            contract.InstrumentBloombergTicker = order.Instrument.Identifiers.BloombergTicker;

            contract.InstrumentUnderlyingName = order.Instrument.UnderlyingName;
            contract.InstrumentUnderlyingCfi = order.Instrument.UnderlyingCfi;
            contract.InstrumentUnderlyingIssuerIdentifier = order.Instrument.UnderlyingIssuerIdentifier;
            contract.InstrumentUnderlyingClientIdentifier = order.Instrument.Identifiers.UnderlyingClientIdentifier;
            contract.InstrumentUnderlyingSedol = order.Instrument.Identifiers.UnderlyingSedol;
            contract.InstrumentUnderlyingIsin = order.Instrument.Identifiers.UnderlyingIsin;
            contract.InstrumentUnderlyingFigi = order.Instrument.Identifiers.UnderlyingFigi;
            contract.InstrumentUnderlyingCusip = order.Instrument.Identifiers.UnderlyingCusip;
            contract.InstrumentUnderlyingLei = order.Instrument.Identifiers.UnderlyingLei;
            contract.InstrumentUnderlyingExchangeSymbol = order.Instrument.Identifiers.UnderlyingExchangeSymbol;
            contract.InstrumentUnderlyingBloombergTicker = order.Instrument.Identifiers.UnderlyingBloombergTicker;

            contract.OrderId = order.OrderId;
            contract.OrderPlacedDate = order.PlacedDate?.ToString("yyyy-MM-ddTHH:mm:ss");
            contract.OrderBookedDate = order.BookedDate?.ToString("yyyy-MM-ddTHH:mm:ss");
            contract.OrderAmendedDate = order.AmendedDate?.ToString("yyyy-MM-ddTHH:mm:ss");
            contract.OrderRejectedDate = order.RejectedDate?.ToString("yyyy-MM-ddTHH:mm:ss");
            contract.OrderCancelledDate = order.CancelledDate?.ToString("yyyy-MM-ddTHH:mm:ss");
            contract.OrderFilledDate = order.FilledDate?.ToString("yyyy-MM-ddTHH:mm:ss");
            contract.OrderType = ((int?) order.OrderType).ToString();
            contract.OrderDirection = ((int?) order.OrderDirection).ToString();
            contract.OrderCurrency = order.OrderCurrency.Code;
            contract.OrderLimitPrice = order.OrderLimitPrice?.Value.ToString();
            contract.OrderAverageFillPrice = order.OrderAverageFillPrice?.Value.ToString();
            contract.OrderOrderedVolume = order.OrderOrderedVolume?.ToString();
            contract.OrderFilledVolume = order.OrderFilledVolume?.ToString();
            contract.OrderTraderId = order.OrderTraderId;
            contract.OrderTraderName = order.OrderTraderName;
            contract.OrderClearingAgent = order.OrderClearingAgent;
            contract.OrderDealingInstructions = order.OrderDealingInstructions;

            return contract;
        }

        /// <summary>
        /// Assumption => csv is validated
        /// </summary>
        public Order Map(OrderFileContract contract)
        {
            if (contract == null)
            {
                FailedParseTotal += 1;
                return null;
            }
            
            var trade = MapTrade(contract);
            var trades = trade != null ? new[] {trade} : null;
            var order = MapOrder(contract, trades);

            return order;
        }

        public int FailedParseTotal { get; set; }

        private Order MapOrder(OrderFileContract contract, IReadOnlyCollection<DealerOrder> dealerOrder)
        {
            if (dealerOrder == null)
            {
                dealerOrder = new DealerOrder[0];
            }

            var instrument = MapInstrument(contract);
            var market = MapMarket(contract);

            var placedDate = MapDate(contract.OrderPlacedDate);
            var bookedDate = MapDate(contract.OrderBookedDate);
            var amendedDate = MapDate(contract.OrderAmendedDate);
            var rejectedDate = MapDate(contract.OrderRejectedDate);
            var cancelledDate = MapDate(contract.OrderCancelledDate);
            var filledDate = MapDate(contract.OrderFilledDate);

            var orderType = MapToEnum<OrderTypes>(contract.OrderType);
            var orderDirection = MapToEnum<OrderDirections>(contract.OrderDirection);
            var orderCurrency = new Currency(contract.OrderCurrency);
            var orderSettlementCurrency =
                !string.IsNullOrWhiteSpace(contract.OrderSettlementCurrency) 
                    ? (Currency?)new Currency(contract.OrderSettlementCurrency) 
                    : null;

            var orderCleanDirty = MapToEnum<OrderCleanDirty>(contract.OrderCleanDirty);

            var limitPrice = new Money(MapDecimal(contract.OrderLimitPrice), contract.OrderCurrency);
            var averagePrice = new Money(MapDecimal(contract.OrderAverageFillPrice), contract.OrderCurrency);

            var orderOptionStrikePrice = new Money(MapDecimal(contract.OrderOptionStrikePrice), contract.OrderCurrency);
            var orderOptionExpirationDate = MapDate(contract.OrderOptionExpirationDate);
            var orderOptionEuropeanAmerican = MapToEnum<OptionEuropeanAmerican>(contract.OrderOptionEuropeanAmerican);

            var orderedVolume = MapLongWithRounding(contract.OrderOrderedVolume);
            var filledVolume = MapLongWithRounding(contract.OrderFilledVolume);

            var accInterest = MapDecimal(contract.OrderAccumulatedInterest);
            
            return new Order(
                instrument,
                market,
                null,
                contract.OrderId,
                DateTime.UtcNow,
                contract.OrderVersion,
                contract.OrderVersionLinkId,
                contract.OrderGroupId,
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
                contract.OrderTraderId,
                contract.OrderTraderName,
                contract.OrderClearingAgent,
                contract.OrderDealingInstructions,
                orderOptionStrikePrice,
                orderOptionExpirationDate,
                orderOptionEuropeanAmerican,
                dealerOrder);
        }

        private DealerOrder MapTrade(OrderFileContract contract)
        {
            if (string.IsNullOrWhiteSpace(contract.DealerOrderId))
                return null;

            var instrument = MapInstrument(contract);

            var placedDate = MapDate(contract.DealerOrderPlacedDate);
            var bookedDate = MapDate(contract.DealerOrderBookedDate);
            var amendedDate = MapDate(contract.DealerOrderAmendedDate);
            var rejectedDate = MapDate(contract.DealerOrderRejectedDate);
            var cancelledDate = MapDate(contract.DealerOrderCancelledDate);
            var filledDate = MapDate(contract.DealerOrderFilledDate);

            var dealerOrderType = MapToEnum<OrderTypes>(contract.DealerOrderType);
            var dealerOrderDirection = MapToEnum<OrderDirections>(contract.DealerOrderDirection);
            var dealerOrderCurrency = new Currency(contract.DealerOrderCurrency);
            var dealerOrderSettlementCurrency = new Currency(contract.DealerOrderSettlementCurrency);

            var limitPrice = new Money(MapDecimal(contract.DealerOrderLimitPrice), contract.DealerOrderCurrency);
            var averagePrice = new Money(MapDecimal(contract.DealerOrderAverageFillPrice), contract.DealerOrderCurrency);
            var cleanDirty = MapToEnum<OrderCleanDirty>(contract.DealerOrderCleanDirty);
            var euroAmerican = MapToEnum<OptionEuropeanAmerican>(contract.DealerOrderOptionEuropeanAmerican);

            var orderedVolume = MapLongWithRounding(contract.DealerOrderOrderedVolume);
            var filledVolume = MapLongWithRounding(contract.DealerOrderFilledVolume);

            var accumulatedInterest = MapDecimal(contract.DealerOrderAccumulatedInterest);
            var optionStrikePrice = MapDecimal(contract.DealerOrderOptionStrikePrice);
            var optionExpirationDate = MapDate(contract.DealerOrderOptionExpirationDate);

            return new DealerOrder(
                instrument,
                string.Empty,
                contract.DealerOrderId,
                placedDate,
                bookedDate,
                amendedDate,
                rejectedDate,
                cancelledDate,
                filledDate,
                DateTime.UtcNow,
                contract.DealerOrderDealerId,
                contract.DealerOrderDealerName,
                contract.DealerOrderNotes,
                contract.DealerOrderCounterParty,
                dealerOrderType,
                dealerOrderDirection,
                dealerOrderCurrency,
                dealerOrderSettlementCurrency,
                cleanDirty,
                accumulatedInterest,
                contract.DealerOrderVersion,
                contract.DealerOrderVersionLinkId,
                contract.DealerOrderGroupId,
                limitPrice,
                averagePrice,
                orderedVolume,
                filledVolume,
                optionStrikePrice,
                optionExpirationDate,
                euroAmerican);
        }

        private T MapToEnum<T>(string propertyValue) where T : struct
        {
            propertyValue = propertyValue?.ToUpper() ?? string.Empty;

            EnumExtensions.TryParsePermutations(propertyValue, out T result);

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

        private long? MapLongWithRounding(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var success = decimal.TryParse(value, out var result);

            if (!success)
                return null;

            var adjustedToIntegerValue = Math.Round(result, 0, MidpointRounding.AwayFromZero);

            return MapLong(adjustedToIntegerValue.ToString());
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

        private FinancialInstrument MapInstrument(OrderFileContract contract)
        {
            var identifiers =
                new InstrumentIdentifiers(
                    string.Empty,
                    string.Empty,
                    null,
                    contract.InstrumentClientIdentifier,
                    AdjustTruncatedSedols(contract.InstrumentSedol),
                    contract.InstrumentIsin,
                    contract.InstrumentFigi,
                    contract.InstrumentCusip,
                    contract.InstrumentExchangeSymbol,
                    contract.InstrumentLei,
                    contract.InstrumentBloombergTicker,
                    contract.InstrumentUnderlyingSedol,
                    contract.InstrumentUnderlyingIsin,
                    contract.InstrumentUnderlyingFigi,
                    contract.InstrumentUnderlyingCusip,
                    contract.InstrumentUnderlyingLei,
                    contract.InstrumentUnderlyingExchangeSymbol,
                    contract.InstrumentUnderlyingBloombergTicker,
                    contract.InstrumentUnderlyingClientIdentifier);

            return new FinancialInstrument(
                MapCfi(contract.InstrumentCfi),
                identifiers,
                contract.InstrumentName ?? string.Empty,
                contract.InstrumentCfi ?? string.Empty,
                contract.OrderCurrency ?? contract.DealerOrderCurrency ?? string.Empty,
                contract.InstrumentIssuerIdentifier ?? string.Empty,
                contract.InstrumentUnderlyingName ?? string.Empty,
                contract.InstrumentUnderlyingCfi ?? string.Empty,
                contract.InstrumentUnderlyingIssuerIdentifier ?? string.Empty);
        }

        /// <summary>
        /// Excel has a tendency to truncate leading 0s on valid sedols.
        /// Insert these in if this is the case
        /// </summary>
        private string AdjustTruncatedSedols(string sedol)
        {
            if (string.IsNullOrWhiteSpace(sedol))
            {
                return sedol;
            }

            while (sedol.Length < 7)
            {
                sedol = $"0{sedol}";
            }

            return sedol;
        }

        private Market MapMarket(OrderFileContract contract)
        {
            Enum.TryParse(contract.MarketType, out MarketTypes marketType);

            return new Market(string.Empty, contract.MarketIdentifierCode, contract.MarketName, marketType);
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
