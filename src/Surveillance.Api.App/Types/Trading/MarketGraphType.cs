using GraphQL.Authorization;
using GraphQL.Types;
using Surveillance.Api.App.Authorization;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.App.Types.Trading
{
    public class MarketGraphType : ObjectGraphType<IMarket>
    {
        public MarketGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);
            Field(i => i.Id).Description("Primary key for the market");
            Field(i => i.MarketName).Description("Name of the market");
            Field(i => i.MarketId).Description("MIC");
        }
    }
}
