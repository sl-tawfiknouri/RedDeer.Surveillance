using GraphQL.Types;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.App.Types.Trading
{
    public class OrderManagementSystemGraphType : ObjectGraphType<IOrderManagementSystem>
    {
        public OrderManagementSystemGraphType()
        {
            Field(i => i.Version).Description("Order version");
            Field(i => i.VersionLinkId).Description("Order version link id");
            Field(i => i.GroupId).Description("Order group id");
        }
    }
}
