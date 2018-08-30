using Domain.Equity.Trading.Orders;
using Surveillance.ElasticSearchDtos.Trades;
using Surveillance.Factories;
using Surveillance.Recorders.Projectors.Interfaces;
using System;

namespace Surveillance.Recorders.Projectors
{
    public class ReddeerTradeFormatProjector : IReddeerTradeFormatProjector
    {
        private IOriginFactory _originFactory;

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
                MarketId = order.Market?.Id?.Id?.ToString() ?? string.Empty,
                MarketName = order.Market?.Name ?? string.Empty,
                SecurityId = order.Security?.Id?.Id ?? string.Empty,
                SecurityName = order.Security?.Name ?? string.Empty,
                Limit = order.Limit?.Value,
                StatusChangedOn = order.StatusChangedOn,
                Volume = order.Volume,
                OrderDirection = order.Direction.ToString(),
                OrderStatusId = (int)order.OrderStatus,
                OrderStatusDescription = order.OrderStatus.ToString()
            };
        }

        private string GenerateDate()
        {
            var id = Guid.NewGuid().ToString();
            id += "." + DateTime.UtcNow.ToString();

            return id;
        }
    }
}
