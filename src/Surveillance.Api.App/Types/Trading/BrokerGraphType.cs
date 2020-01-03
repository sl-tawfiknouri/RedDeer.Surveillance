namespace Surveillance.Api.App.Types.Trading
{
    using GraphQL.Authorization;
    using GraphQL.Types;

    using Surveillance.Api.App.Authorization;
    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class BrokerGraphType : ObjectGraphType<IBroker>
    {
        public BrokerGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);
            this.Field(i => i.Id).Description("Primary key for the broker");
            this.Field(i => i.Name).Description("Name of the broker");
            this.Field(i => i.ExternalId, nullable: true).Description("External Id of broker.");
            this.Field(i => i.CreatedOn).Type(new DateTimeGraphType()).Description("CreatedOn DateTime.");
        }
    }
}