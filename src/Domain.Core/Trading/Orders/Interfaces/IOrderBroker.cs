using System;

namespace Domain.Core.Trading.Orders.Interfaces
{
    public interface IOrderBroker
    {
        DateTime CreatedOn { get; }
        string Id { get; }
        bool Live { get; }
        string Name { get; }
        string ReddeerId { get; }
    }
}