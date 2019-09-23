namespace Surveillance.Api.App.Types.Trading
{
    using Domain.Core.Trading.Orders;

    using GraphQL.Authorization;
    using GraphQL.Types;

    using Surveillance.Api.App.Authorization;

    public class OrderDirectionGraphType : EnumerationGraphType<OrderDirections>
    {
        public OrderDirectionGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);
            this.Name = "OrderDirections";
        }
    }
}