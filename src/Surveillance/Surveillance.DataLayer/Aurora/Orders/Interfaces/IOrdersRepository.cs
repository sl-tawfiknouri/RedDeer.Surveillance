using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Core.Trading.Orders;
using Surveillance.Auditing.Context.Interfaces;

namespace Surveillance.DataLayer.Aurora.Orders.Interfaces
{
    public interface IOrdersRepository
    {
        Task Create(Order entity);
        Task<IReadOnlyCollection<Order>> Get(DateTime start, DateTime end, ISystemProcessOperationContext opCtx);
        Task<IReadOnlyCollection<Order>> LiveUnscheduledOrders();
        Task SetOrdersScheduled(IReadOnlyCollection<Order> orders);
        Task LivenCompletedOrderSets();
        Task<IReadOnlyCollection<Order>> StaleOrders(DateTime stalenessDate);
    }
}