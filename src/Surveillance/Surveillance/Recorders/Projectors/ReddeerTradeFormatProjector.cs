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
                SecurityName = order.Security?.Name ?? string.Empty,

                SecurityClientIdentifier = order.Security?.Identifiers.ClientIdentifier,
                SecuritySedol = order.Security?.Identifiers.Sedol,
                SecurityIsin = order.Security?.Identifiers.Isin,
                SecurityFigi = order.Security?.Identifiers.Figi,

                Limit = order.Limit?.Value,
                LimitCurrency = order.Limit?.Currency,

                StatusChangedOn = order.StatusChangedOn,
                Volume = order.Volume,
                OrderDirectionId = (int)order.Direction,
                OrderDirectionDescription = order.Direction.ToString(),
                OrderStatusId = (int)order.OrderStatus,
                OrderStatusDescription = order.OrderStatus.ToString()
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
