using Domain.Trades.Orders;
using RedDeer.Contracts.SurveillanceService.TradeOrder;
using Surveillance.Mappers.Interfaces;

namespace Surveillance.Mappers
{
    public class TradeOrderDataItemDtoMapper : ITradeOrderDataItemDtoMapper
    {
        public TradeOrderDataItemDto Map(TradeOrderFrame frame)
        {
            if (frame == null)
            {
                return null;
            }

            return new TradeOrderDataItemDto
            {
                OrderType = (TradeOrderType)frame.OrderType,
                MarketIdentifierCode = frame.Market?.Id?.Id,
                MarketName = frame.Market?.Name,
                SecurityName = frame.Security?.Name,
                SecurityClientIdentifier = frame.Security?.Identifiers.ClientIdentifier,
                SecuritySedol = frame.Security?.Identifiers.Sedol,
                SecurityIsin = frame.Security?.Identifiers.Isin,
                SecurityFigi = frame.Security?.Identifiers.Figi,
                SecurityCusip = frame.Security?.Identifiers.Cusip,
                SecurityExchangeSymbol = frame.Security?.Identifiers.ExchangeSymbol,
                SecurityCfi = frame.Security?.Cfi,
                LimitPrice = frame.Limit?.Value,
                TradeSubmittedOn = frame.TradeSubmittedOn,
                StatusChangedOn = frame.StatusChangedOn,
                Volume = frame.Volume,
                OrderPosition = (TradeOrderPosition)frame.Position,
                OrderStatus = (TradeOrderStatus)frame.OrderStatus,
                TraderId = frame.TraderId,
                ClientAttributionId = frame.TradeClientAttributionId,
                PartyBrokerId = frame.PartyBrokerId,
                CounterPartyBrokerId = frame.CounterPartyBrokerId
            };
        }
    }
}