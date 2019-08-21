namespace Surveillance.Api.App.Types.Trading
{
    using GraphQL.Authorization;
    using GraphQL.Types;

    using Surveillance.Api.App.Authorization;
    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class MarketGraphType : ObjectGraphType<IMarket>
    {
        public MarketGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);
            this.Field(i => i.Id).Description("Primary key for the market");
            this.Field(i => i.MarketName).Description("Name of the market");
            this.Field(i => i.MarketId).Description("MIC");
        }
    }
}