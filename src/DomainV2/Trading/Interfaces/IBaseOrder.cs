using DomainV2.Financial;

namespace DomainV2.Trading.Interfaces
{
    public interface IBaseOrder
    {
        OrderStatus OrderStatus();
    }
}