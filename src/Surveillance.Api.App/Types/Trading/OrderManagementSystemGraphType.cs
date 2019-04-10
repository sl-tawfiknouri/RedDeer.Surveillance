using GraphQL.Authorization;
using GraphQL.Types;
using Surveillance.Api.App.Authorization;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.App.Types.Trading
{
    public class OrderManagementSystemGraphType : ObjectGraphType<IOrderManagementSystem>
    {
        public OrderManagementSystemGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);
            Field(i => i.Version).Description("Order version");
            Field(i => i.VersionLinkId).Description("Order version link id");
            Field(i => i.GroupId).Description("Order group id");
        }
    }
}
