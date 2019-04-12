using Domain.Core.Trading.Orders;
using GraphQL.Authorization;
using GraphQL.Types;
using Surveillance.Api.App.Authorization;

namespace Surveillance.Api.App.Types.Trading
{
    public class OrderTypeGraphType : EnumerationGraphType<OrderTypes>
    {
        public OrderTypeGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            Name = "OrderTypes";
        }
    }
}
