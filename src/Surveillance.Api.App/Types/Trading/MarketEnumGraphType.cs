using Domain.Core.Markets;
using GraphQL.Authorization;
using GraphQL.Types;
using Surveillance.Api.App.Authorization;

namespace Surveillance.Api.App.Types.Trading
{
    public class MarketEnumGraphType : EnumerationGraphType<MarketTypes>
    {
        public MarketEnumGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);
            Name = "MarketCategory";
        }
    }
}
