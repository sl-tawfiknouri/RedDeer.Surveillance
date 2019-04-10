using Domain.Core.Trading.Orders;
using GraphQL.Types;

namespace Surveillance.Api.App.Types.Trading
{
    public class OrderTypeGraphType : EnumerationGraphType<OrderTypes>
    {
        public OrderTypeGraphType()
        {
            Name = "OrderTypes";
        }
    }
}
