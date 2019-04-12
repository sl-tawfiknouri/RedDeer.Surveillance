using Domain.Core.Trading.Interfaces;
using GraphQL.Authorization;
using GraphQL.Types;
using Surveillance.Api.App.Authorization;

namespace Surveillance.Api.App.Types.Trading
{
    public class OrderLedgerGraphType : ObjectGraphType<IOrderLedger>
    {
        public OrderLedgerGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);
            Field<ListGraphType<OrderGraphType>>().Name("orders").Description("The orders associated with this portfolio");
        }
    }
}
