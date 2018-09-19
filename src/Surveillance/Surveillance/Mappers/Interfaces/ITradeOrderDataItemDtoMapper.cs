using Domain.Trades.Orders;
using RedDeer.Contracts.SurveillanceService.TradeOrder;

namespace Surveillance.Mappers.Interfaces
{
    public interface ITradeOrderDataItemDtoMapper
    {
        TradeOrderDataItemDto Map(TradeOrderFrame frame);
    }
}