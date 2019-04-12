using GraphQL.Authorization;
using GraphQL.Types;
using Surveillance.Api.App.Authorization;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.App.Types.Trading
{
    public class TraderGraphType : ObjectGraphType<ITrader>
    {
        public TraderGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            Name = "Trader";

            Field(i => i.Id).Description("The identifier for the trader provided in the orders file");
            Field(i => i.Name).Description("The name associated with the trader");
        }
    }
}
