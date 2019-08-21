namespace Surveillance.Api.App.Types.Trading
{
    using Domain.Core.Trading.Orders;

    using GraphQL.Authorization;
    using GraphQL.Types;

    using Surveillance.Api.App.Authorization;

    public class OrderTypeGraphType : EnumerationGraphType<OrderTypes>
    {
        public OrderTypeGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            this.Name = "OrderTypes";
        }
    }
}