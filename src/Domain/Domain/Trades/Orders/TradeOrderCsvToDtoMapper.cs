using System;
using Domain.Equity;
using Domain.Market;
using Domain.Trades.Orders.Interfaces;
using Microsoft.Extensions.Logging;

namespace Domain.Trades.Orders
{
    public class TradeOrderCsvToDtoMapper : ITradeOrderCsvToDtoMapper
    {
        private readonly ILogger<TradeOrderCsvToDtoMapper> _logger;

        public TradeOrderCsvToDtoMapper()
        { }

        // ReSharper disable once UnusedMember.Global
        public TradeOrderCsvToDtoMapper(ILogger<TradeOrderCsvToDtoMapper> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public int FailedParseTotal { get; set; }

        public TradeOrderFrame Map(TradeOrderFrameCsv csv)
        {
            if (csv == null)
            {
                FailedParseTotal += 1;
                _logger?.LogError("Failed to parse trade order frame csv due to being passed a null value");

                return null;
            }

            if (!Enum.TryParse(csv.OrderPosition, out OrderPosition orderPosition))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable order position {csv.OrderPosition} for row {csv.RowId}");

                return null;
            }

            if (!Enum.TryParse(csv.OrderType, out OrderType orderType))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable order type {csv.OrderType} for row {csv.RowId}");

                return null;
            }

            if (!Enum.TryParse(csv.OrderStatus, out OrderStatus orderStatus))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable order status {csv.OrderStatus} for row {csv.RowId}");

                return null;
            }

            if (!int.TryParse(csv.FulfilledVolume, out var fulfilledVolume))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable fulfilled volume {csv.FulfilledVolume} for row {csv.RowId}");

                return null;
            }

            if (!int.TryParse(csv.OrderedVolume, out var orderedVolume))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable ordered volume {csv.OrderedVolume} for row {csv.RowId}");

                return null;
            }

            if (!decimal.TryParse(csv.LimitPrice, out var limitPrice)
                && orderType == OrderType.Limit)
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable limit price on a limit order {csv.LimitPrice} for row {csv.RowId}");

                return null;
            }

            var parsedLimitPrice =
                orderType == OrderType.Limit
                    ? (decimal?)limitPrice
                    : null;

            var pricedLimitPrice =
                parsedLimitPrice != null
                    ? (Price?)new Price(parsedLimitPrice.Value, csv.OrderCurrency)
                    : null;

            // ReSharper disable once InlineOutVariableDeclaration
            decimal executedPrice = 0;
            if (!string.IsNullOrWhiteSpace(csv.ExecutedPrice)
                && !decimal.TryParse(csv.ExecutedPrice, out executedPrice))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable executed price {csv.ExecutedPrice} for row {csv.RowId}");

                return null;
            }

            var pricedExecutedPrice =
                !string.IsNullOrWhiteSpace(csv.ExecutedPrice)
                    ? (Price?)new Price(executedPrice, csv.OrderCurrency)
                    : null;

            if (!DateTime.TryParse(csv.StatusChangedOn, out var statusChangedOn))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable status changed on date {csv.StatusChangedOn} for row {csv.RowId}");

                return null;
            }

            if (!DateTime.TryParse(csv.TradeSubmittedOn, out var tradeSubmittedOn))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable trade submitted on date {csv.TradeSubmittedOn} for row {csv.RowId}");

                return null;
            }

            var market = new StockExchange(new Market.Market.MarketId(csv.MarketIdentifierCode), csv.MarketName);

            var securityIdentifiers =
                new SecurityIdentifiers(
                    csv.SecurityClientIdentifier,
                    csv.SecuritySedol,
                    csv.SecurityIsin,
                    csv.SecurityFigi,
                    csv.SecurityCusip,
                    csv.SecurityExchangeSymbol,
                    csv.SecurityLei,
                    csv.SecurityBloombergTicker);

            var security =
                new Security(
                    securityIdentifiers,
                    csv.SecurityName,
                    csv.SecurityCfi,
                    csv.SecurityIssuerIdentifier);

            return new TradeOrderFrame(
                orderType,
                market,
                security,
                pricedLimitPrice,
                pricedExecutedPrice,
                fulfilledVolume,
                orderedVolume,
                orderPosition,
                orderStatus,
                statusChangedOn,
                tradeSubmittedOn,
                csv.TraderId,
                csv.ClientAttributionId,
                csv.AccountId,
                csv.DealerInstructions,
                csv.PartyBrokerId,
                csv.CounterPartyBrokerId,
                csv.TradeRationale,
                csv.TradeStrategy,
                csv.OrderCurrency);
        }
    }
}