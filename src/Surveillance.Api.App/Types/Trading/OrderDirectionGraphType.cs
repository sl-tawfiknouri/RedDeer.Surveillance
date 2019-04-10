using Domain.Core.Trading.Orders;
using GraphQL.Types;

namespace Surveillance.Api.App.Types.Trading
{
    public class OrderDirectionGraphType : EnumerationGraphType<OrderDirections>
    {
        public OrderDirectionGraphType()
        {
            Name = "OrderDirections";
        }
    }
}
