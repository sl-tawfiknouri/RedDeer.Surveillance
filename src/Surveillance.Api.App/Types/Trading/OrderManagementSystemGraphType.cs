namespace Surveillance.Api.App.Types.Trading
{
    using GraphQL.Authorization;
    using GraphQL.Types;

    using Surveillance.Api.App.Authorization;
    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class OrderManagementSystemGraphType : ObjectGraphType<IOrderManagementSystem>
    {
        public OrderManagementSystemGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);
            this.Field(i => i.Version).Description("Order version");
            this.Field(i => i.VersionLinkId).Description("Order version link id");
            this.Field(i => i.GroupId).Description("Order group id");
        }
    }
}