using Domain.Core.Trading.Interfaces;
using GraphQL.Types;

namespace Surveillance.Api.App.Types.Trading
{
    public class OrderLedgerGraphType : ObjectGraphType<IOrderLedger>
    {
        public OrderLedgerGraphType()
        {
            Field<ListGraphType<OrderGraphType>>().Name("Orders").Description("The orders associated with this portfolio");
        }
    }
}
