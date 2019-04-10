using Domain.Core.Markets;
using GraphQL.Types;

namespace Surveillance.Api.App.Types.Trading
{
    public class MarketEnumGraphType : EnumerationGraphType<MarketTypes>
    {
        public MarketEnumGraphType()
        {
            Name = "MarketCategory";
        }
    }
}
