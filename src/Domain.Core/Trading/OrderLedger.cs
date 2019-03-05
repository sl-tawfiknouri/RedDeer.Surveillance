using Domain.Trading;
using System.Collections.Generic;

namespace Domain.Core.Trading
{
    public class OrderLedger : IOrderLedger
    {
        public void Add(Order order)
        {

        }

        public IReadOnlyCollection<Order> FullLedger()
        {
            return new List<Order>();
        }
    }
}
