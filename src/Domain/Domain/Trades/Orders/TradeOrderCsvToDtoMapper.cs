using System;
using System.Globalization;
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

            var failedParse = false;

            if (!Enum.TryParse(csv.OrderPosition, out OrderPosition orderPosition))
            {
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable order position {csv.OrderPosition} for row {csv.RowId}");
                failedParse = true;
            }

            if (!Enum.TryParse(csv.OrderType, out OrderType orderType))
            {
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable order type {csv.OrderType} for row {csv.RowId}");
                failedParse = true;
            }

            if (!Enum.TryParse(csv.OrderStatus, out OrderStatus orderStatus))
            {
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable order status {csv.OrderStatus} for row {csv.RowId}");
                failedParse = true;
            }

            if (!int.TryParse(csv.FulfilledVolume, out var fulfilledVolume))
            {
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable fulfilled volume {csv.FulfilledVolume} for row {csv.RowId}");
                failedParse = true;
            }

            if (!int.TryParse(csv.OrderedVolume, out var orderedVolume))
            {
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable ordered volume {csv.OrderedVolume} for row {csv.RowId}");
                failedParse = true;
            }

            if (!decimal.TryParse(csv.LimitPrice, out var limitPrice)
                && orderType == OrderType.Limit)
            {
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable limit price on a limit order {csv.LimitPrice} for row {csv.RowId}");
                failedParse = true;
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
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable executed price {csv.ExecutedPrice} for row {csv.RowId}");
                failedParse = true;
            }

            var pricedExecutedPrice =
                !string.IsNullOrWhiteSpace(csv.ExecutedPrice)
                    ? (Price?)new Price(executedPrice, csv.OrderCurrency)
                    : null;

            if (!DateTime.TryParse(csv.StatusChangedOn, out var statusChangedOn))
            {
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable status changed on date {csv.StatusChangedOn} for row {csv.RowId}");
                failedParse = true;
            }

            if (!DateTime.TryParse(csv.TradeSubmittedOn, out var tradeSubmittedOn))
            {
                _logger?.LogError($"Failed to parse trade order frame csv due to being passed an unparseable trade submitted on date {csv.TradeSubmittedOn} for row {csv.RowId}");
                failedParse = true;
            }

            if (failedParse)
            {
                FailedParseTotal += 1;
                return null;
            }

            var market = new StockExchange(new Market.Market.MarketId(csv.MarketIdentifierCode), csv.MarketName);

            var securityIdentifiers =
                new SecurityIdentifiers(
                    string.Empty,
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

        public TradeOrderFrameCsv Map(TradeOrderFrame frame)
        {
            if (frame == null)
            {
                return new TradeOrderFrameCsv();
            }

            return new TradeOrderFrameCsv
            {
                OrderType = frame.OrderType.ToString(),
                MarketIdentifierCode = frame.Market?.Id.Id,
                MarketName = frame.Market?.Name,
                SecurityName = frame.Security?.Name,
                SecurityCfi =  frame.Security?.Cfi,
                SecurityIssuerIdentifier = frame.Security?.IssuerIdentifier,
                SecurityClientIdentifier = frame.Security?.Identifiers.ClientIdentifier,
                SecuritySedol = frame.Security?.Identifiers.Sedol,
                SecurityIsin = frame.Security?.Identifiers.Isin,
                SecurityFigi = frame.Security?.Identifiers.Figi,
                SecurityCusip = frame.Security?.Identifiers.Cusip,
                SecurityLei = frame.Security?.Identifiers.Lei,
                SecurityExchangeSymbol = frame.Security?.Identifiers.ExchangeSymbol,
                SecurityBloombergTicker = frame.Security?.Identifiers.BloombergTicker,
                LimitPrice = frame.Limit?.Value.ToString() ?? null,
                ExecutedPrice = frame.ExecutedPrice?.Value.ToString() ?? null,
                TradeSubmittedOn = frame.TradeSubmittedOn.ToString(CultureInfo.CurrentCulture),
                StatusChangedOn = frame.StatusChangedOn.ToString(CultureInfo.CurrentCulture),
                OrderedVolume = frame.OrderedVolume.ToString(),
                FulfilledVolume = frame.FulfilledVolume.ToString(),
                OrderPosition = frame.Position.ToString(),
                TraderId = frame.TraderId,
                ClientAttributionId = frame.TradeClientAttributionId,
                AccountId = frame.AccountId,
                PartyBrokerId = frame.PartyBrokerId,
                CounterPartyBrokerId = frame.CounterPartyBrokerId,
                DealerInstructions = frame.DealerInstructions,
                TradeRationale = frame.TradeRationale,
                TradeStrategy = frame.TradeStrategy,
                OrderStatus = frame.OrderStatus.ToString(),
                OrderCurrency = frame.OrderCurrency
            };
        }
    }
}