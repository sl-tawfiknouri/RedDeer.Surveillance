using Surveillance.ElasticSearchDtos.Trades;
using Surveillance.Factories.Interfaces;
using Surveillance.Recorders.Projectors.Interfaces;
using System;
using Domain.Trades.Orders;

namespace Surveillance.Recorders.Projectors
{
    public class ReddeerTradeFormatProjector : IReddeerTradeFormatProjector
    {
        private readonly IOriginFactory _originFactory;

        public ReddeerTradeFormatProjector(IOriginFactory originFactory)
        {
            _originFactory = originFactory ?? throw new ArgumentNullException(nameof(originFactory));
        }

        public ReddeerTradeDocument Project(TradeOrderFrame order)
        {
            if (order == null)
            {
                return null;
            }

            return new ReddeerTradeDocument
            {
                Id = GenerateDate(),
                Origin = _originFactory.Origin(),
                OrderTypeId = (int)order.OrderType,
                OrderTypeDescription = order.OrderType.ToString(),

                MarketId = order.Market?.Id?.Id ?? string.Empty,
                MarketName = order.Market?.Name ?? string.Empty,

                SecurityClientIdentifier = order.Security?.Identifiers.ClientIdentifier,
                SecuritySedol = order.Security?.Identifiers.Sedol,
                SecurityIsin = order.Security?.Identifiers.Isin,
                SecurityFigi = order.Security?.Identifiers.Figi,
                SecurityCusip = order.Security?.Identifiers.Cusip,
                SecurityExchangeSymbol = order.Security?.Identifiers.ExchangeSymbol,

                SecurityName = order.Security?.Name ?? string.Empty,
                SecurityCfi = order.Security?.Cfi,

                Limit = order.Limit?.Value,
                LimitCurrency = order.Limit?.Currency,

                StatusChangedOn = order.StatusChangedOn,
                TradeSubmittedOn = order.TradeSubmittedOn,

                FulfilledVolume = order.FulfilledVolume,
                
                OrderPositionId = (int)order.Position,
                OrderPositionDescription = order.Position.ToString(),

                OrderStatusId = (int)order.OrderStatus,
                OrderStatusDescription = order.OrderStatus.ToString(),

                TraderId = order.TraderId,
                TradeClientAttributionId = order.TradeClientAttributionId,
                PartyBrokerId = order.PartyBrokerId,
                CounterPartyBrokerId = order.CounterPartyBrokerId,

                SecurityIssuerIdentifier = order.Security?.IssuerIdentifier,
                SecurityLei = order.Security?.Identifiers.Lei,
                SecurityBloombergTicker = order.Security?.Identifiers.BloombergTicker,
                ExecutedPrice = order.ExecutedPrice?.Value,
                OrderedVolume = order.OrderedVolume,
                AccountId = order.AccountId,
                DealerInstructions = order.DealerInstructions,
                TradeRationale = order.TradeRationale,
                TradeStrategy = order.TradeStrategy
            };
        }

        private string GenerateDate()
        {
            var id = Guid.NewGuid().ToString();
            id += "." + DateTime.UtcNow;

            return id;
        }
    }
}
