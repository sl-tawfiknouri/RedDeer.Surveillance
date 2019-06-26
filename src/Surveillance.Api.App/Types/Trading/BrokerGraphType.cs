using GraphQL.Authorization;
using GraphQL.Types;
using Surveillance.Api.App.Authorization;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.App.Types.Trading
{
    public class BrokerGraphType : ObjectGraphType<IBroker>
    {
        public BrokerGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);
            Field(i => i.Id).Description("Primary key for the broker");
            Field(i => i.Name).Description("Name of the broker");
            Field(i => i.ExternalId).Description("External Id of broker.");
            Field(i => i.CreatedOn).Type(new DateTimeGraphType()).Description("CreatedOn DateTime.");
        }
    }
}
