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
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable order position {orderPosition}");

                return null;
            }

            if (!Enum.TryParse(csv.OrderType, out OrderType orderType))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable order type {orderType}");

                return null;
            }

            if (!Enum.TryParse(csv.OrderStatus, out OrderStatus orderStatus))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable order status {orderStatus}");

                return null;
            }

            if (!int.TryParse(csv.Volume, out var volume))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable volume {volume}");

                return null;
            }

            if (!decimal.TryParse(csv.LimitPrice, out var limitPrice)
                && orderType == OrderType.Limit)
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable limit price on a limit order {limitPrice}");

                return null;
            }

            var parsedLimitPrice =
                orderType == OrderType.Limit
                    ? (decimal?)limitPrice
                    : null;

            var pricedLimitPrice =
                parsedLimitPrice != null
                    ? (Price?)new Price(parsedLimitPrice.Value, csv.Currency)
                    : null; 

            if (!DateTime.TryParse(csv.StatusChangedOn, out var statusChangedOn))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable status changed on date {statusChangedOn}");

                return null;
            }

            if (!DateTime.TryParse(csv.TradeSubmittedOn, out var tradeSubmittedOn))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable trade submitted on date {tradeSubmittedOn}");

                return null;
            }

            return new TradeOrderFrame(
                orderType,
                new StockExchange(
                    new Market.Market.MarketId(csv.MarketIdentifierCode),
                    csv.MarketName),
                new Security(
                    new SecurityIdentifiers(
                        csv.SecurityClientIdentifier,
                        csv.SecuritySedol,
                        csv.SecurityIsin,
                        csv.SecurityFigi,
                        csv.SecurityCusip,
                        csv.SecurityExchangeSymbol),
                    csv.SecurityName,
                    csv.SecurityCfi),
                pricedLimitPrice,
                volume,
                orderPosition,
                orderStatus,
                statusChangedOn,
                tradeSubmittedOn,
                csv.TraderId,
                csv.ClientAttributionId,
                csv.PartyBrokerId,
                csv.CounterPartyBrokerId);
        }
    }
}