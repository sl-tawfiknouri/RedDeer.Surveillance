using Domain.Core.Financial;

namespace Domain.Trading.Interfaces
{
    public interface IBaseOrder
    {
        OrderStatus OrderStatus();
    }
}