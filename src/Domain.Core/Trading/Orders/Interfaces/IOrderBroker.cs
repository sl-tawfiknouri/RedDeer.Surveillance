using System;

namespace Domain.Core.Trading.Orders.Interfaces
{
    public interface IOrderBroker
    {
        DateTime? CreatedOn { get; set; }
        string Id { get; set; }
        bool Live { get; set; }
        string Name { get; set; }
        string ReddeerId { get; set; }
    }
}