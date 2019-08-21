namespace Surveillance.Api.App.Types.Trading
{
    using Domain.Core.Markets;

    using GraphQL.Authorization;
    using GraphQL.Types;

    using Surveillance.Api.App.Authorization;

    public class MarketEnumGraphType : EnumerationGraphType<MarketTypes>
    {
        public MarketEnumGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);
            this.Name = "MarketCategory";
        }
    }
}