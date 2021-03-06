﻿namespace SharedKernel.Files.Orders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Extensions;
    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Money;
    using Domain.Core.Markets;
    using Domain.Core.Trading.Orders;

    using SharedKernel.Files.Orders.Interfaces;

    public class OrderFileToOrderSerialiser : IOrderFileToOrderSerialiser
    {
        public int FailedParseTotal { get; set; }

        public OrderFileContract[] Map(Order order)
        {
            if (order == null) return null;

            var csv = new OrderFileContract();
            csv = this.MapOrderFields(order, csv);

            if (order.DealerOrders == null || !order.DealerOrders.Any()) return new[] { csv };

            var result = this.MapTradeFields(order);

            return result;
        }

        /// <summary>
        ///     Assumption => csv is validated
        /// </summary>
        public Order Map(OrderFileContract contract)
        {
            if (contract == null)
            {
                this.FailedParseTotal += 1;
                return null;
            }

            var trade = this.MapTrade(contract);
            var trades = trade != null ? new[] { trade } : null;
            var order = this.MapOrder(contract, trades);

            return order;
        }

        /// <summary>
        ///     Excel has a tendency to truncate leading 0s on valid sedols.
        ///     Insert these in if this is the case
        /// </summary>
        private string AdjustTruncatedSedols(string sedol)
        {
            if (string.IsNullOrWhiteSpace(sedol)) 
                return sedol;

            while (sedol.Length < 7) 
                sedol = $"0{sedol}";

            return sedol;
        }

        private InstrumentTypes MapCfi(string cfi)
        {
            if (string.IsNullOrWhiteSpace(cfi)) return InstrumentTypes.None;

            cfi = cfi.ToLower();

            if (cfi.Take(1).ToString() == "e") return InstrumentTypes.Equity;

            if (cfi.Take(2).ToString() == "db") return InstrumentTypes.Bond;

            if (cfi.Take(2).ToString() == "oc") return InstrumentTypes.OptionCall;

            if (cfi.Take(2).ToString() == "op") return InstrumentTypes.OptionPut;

            return InstrumentTypes.Unknown;
        }

        private DateTime? MapDate(string date)
        {
            if (!DateTime.TryParse(date, out var result))
            {
                return null;
            }

            return result;
        }

        private decimal? MapDecimal(string value)
        {
            if (!decimal.TryParse(value, out var result))
            {
                return null;
            }

            return result;
        }

        private FinancialInstrument MapInstrument(OrderFileContract contract)
        {
            var identifiers = new InstrumentIdentifiers(
                string.Empty,
                string.Empty,
                null,
                contract.InstrumentClientIdentifier,
                this.AdjustTruncatedSedols(contract.InstrumentSedol),
                contract.InstrumentIsin,
                contract.InstrumentFigi,
                contract.InstrumentCusip,
                contract.InstrumentExchangeSymbol,
                contract.InstrumentLei,
                contract.InstrumentBloombergTicker,
                contract.InstrumentRic,
                contract.InstrumentUnderlyingSedol,
                contract.InstrumentUnderlyingIsin,
                contract.InstrumentUnderlyingFigi,
                contract.InstrumentUnderlyingCusip,
                contract.InstrumentUnderlyingLei,
                contract.InstrumentUnderlyingExchangeSymbol,
                contract.InstrumentUnderlyingBloombergTicker,
                contract.InstrumentUnderlyingClientIdentifier,
                contract.InstrumentUnderlyingRic);

            return new FinancialInstrument(
                this.MapCfi(contract.InstrumentCfi),
                identifiers,
                contract.InstrumentName ?? string.Empty,
                contract.InstrumentCfi ?? string.Empty,
                contract.OrderCurrency ?? contract.DealerOrderCurrency ?? string.Empty,
                contract.InstrumentIssuerIdentifier ?? string.Empty,
                contract.InstrumentUnderlyingName ?? string.Empty,
                contract.InstrumentUnderlyingCfi ?? string.Empty,
                contract.InstrumentUnderlyingIssuerIdentifier ?? string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty);
        }

        private Market MapMarket(OrderFileContract contract)
        {
            Enum.TryParse(contract.MarketType, out MarketTypes marketType);

            return new Market(string.Empty, contract.MarketIdentifierCode, contract.MarketName, marketType);
        }

        private Order MapOrder(OrderFileContract contract, IReadOnlyCollection<DealerOrder> dealerOrder)
        {
            if (dealerOrder == null) dealerOrder = new DealerOrder[0];

            var instrument = this.MapInstrument(contract);
            var market = this.MapMarket(contract);

            var placedDate = this.MapDate(contract.OrderPlacedDate);
            var bookedDate = this.MapDate(contract.OrderBookedDate);
            var amendedDate = this.MapDate(contract.OrderAmendedDate);
            var rejectedDate = this.MapDate(contract.OrderRejectedDate);
            var cancelledDate = this.MapDate(contract.OrderCancelledDate);
            var filledDate = this.MapDate(contract.OrderFilledDate);

            var orderType = this.MapToEnum<OrderTypes>(contract.OrderType);
            var orderDirection = this.MapToEnum<OrderDirections>(contract.OrderDirection);
            var orderCurrency = new Currency(contract.OrderCurrency);
            var orderSettlementCurrency = !string.IsNullOrWhiteSpace(contract.OrderSettlementCurrency)
                                              ? new Currency(contract.OrderSettlementCurrency)
                                              : (Currency?)null;

            var orderCleanDirty = this.MapToEnum<OrderCleanDirty>(contract.OrderCleanDirty);

            var limitPrice = new Money(this.MapDecimal(contract.OrderLimitPrice), contract.OrderCurrency);
            var averagePrice = new Money(this.MapDecimal(contract.OrderAverageFillPrice), contract.OrderCurrency);

            var orderOptionStrikePrice = new Money(
                this.MapDecimal(contract.OrderOptionStrikePrice),
                contract.OrderCurrency);
            var orderOptionExpirationDate = this.MapDate(contract.OrderOptionExpirationDate);
            var orderOptionEuropeanAmerican =
                this.MapToEnum<OptionEuropeanAmerican>(contract.OrderOptionEuropeanAmerican);

            var orderedVolume = this.MapDecimal(contract.OrderOrderedVolume);
            var filledVolume = this.MapDecimal(contract.OrderFilledVolume);

            var accInterest = this.MapDecimal(contract.OrderAccumulatedInterest);

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
                new OrderBroker(string.Empty, string.Empty, contract.OrderBroker, null, false),
                orderOptionStrikePrice,
                orderOptionExpirationDate,
                orderOptionEuropeanAmerican,
                dealerOrder);
        }

        private OrderFileContract MapOrderFields(Order order, OrderFileContract contract)
        {
            contract.MarketType = ((int?)order.Market.Type)?.ToString();
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
            contract.OrderType = ((int?)order.OrderType).ToString();
            contract.OrderDirection = ((int?)order.OrderDirection).ToString();
            contract.OrderCurrency = order.OrderCurrency.Code;
            contract.OrderLimitPrice = order.OrderLimitPrice?.Value.ToString();
            contract.OrderAverageFillPrice = order.OrderAverageFillPrice?.Value.ToString();
            contract.OrderOrderedVolume = order.OrderOrderedVolume?.ToString();
            contract.OrderFilledVolume = order.OrderFilledVolume?.ToString();
            contract.OrderTraderId = order.OrderTraderId;
            contract.OrderTraderName = order.OrderTraderName;
            contract.OrderClearingAgent = order.OrderClearingAgent;
            contract.OrderDealingInstructions = order.OrderDealingInstructions;
            contract.OrderBroker = order.OrderBroker?.Name;

            return contract;
        }

        private T MapToEnum<T>(string propertyValue)
            where T : struct
        {
            EnumExtensions.TryParsePermutations(propertyValue, out T result);
            return result;
        }

        private DealerOrder MapTrade(OrderFileContract contract)
        {
            if (string.IsNullOrWhiteSpace(contract.DealerOrderId))
                return null;

            var instrument = this.MapInstrument(contract);

            var placedDate = this.MapDate(contract.DealerOrderPlacedDate);
            var bookedDate = this.MapDate(contract.DealerOrderBookedDate);
            var amendedDate = this.MapDate(contract.DealerOrderAmendedDate);
            var rejectedDate = this.MapDate(contract.DealerOrderRejectedDate);
            var cancelledDate = this.MapDate(contract.DealerOrderCancelledDate);
            var filledDate = this.MapDate(contract.DealerOrderFilledDate);

            var dealerOrderType = this.MapToEnum<OrderTypes>(contract.DealerOrderType);
            var dealerOrderDirection = this.MapToEnum<OrderDirections>(contract.DealerOrderDirection);
            var dealerOrderCurrency = new Currency(contract.DealerOrderCurrency);
            var dealerOrderSettlementCurrency = new Currency(contract.DealerOrderSettlementCurrency);

            var limitPrice = new Money(this.MapDecimal(contract.DealerOrderLimitPrice), contract.DealerOrderCurrency);
            var averagePrice = new Money(
                this.MapDecimal(contract.DealerOrderAverageFillPrice),
                contract.DealerOrderCurrency);
            var cleanDirty = this.MapToEnum<OrderCleanDirty>(contract.DealerOrderCleanDirty);
            var euroAmerican = this.MapToEnum<OptionEuropeanAmerican>(contract.DealerOrderOptionEuropeanAmerican);

            var orderedVolume = this.MapDecimal(contract.DealerOrderOrderedVolume);
            var filledVolume = this.MapDecimal(contract.DealerOrderFilledVolume);

            var accumulatedInterest = this.MapDecimal(contract.DealerOrderAccumulatedInterest);
            var optionStrikePrice = this.MapDecimal(contract.DealerOrderOptionStrikePrice);
            var optionExpirationDate = this.MapDate(contract.DealerOrderOptionExpirationDate);

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

        private OrderFileContract[] MapTradeFields(Order order)
        {
            // ReSharper disable once IdentifierTypo
            var csvs = new List<OrderFileContract>();

            foreach (var trad in order.DealerOrders)
            {
                var csv = new OrderFileContract();
                csv = this.MapTradeFields2(trad, csv);
                csv = this.MapOrderFields(order, csv);
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
    }
}