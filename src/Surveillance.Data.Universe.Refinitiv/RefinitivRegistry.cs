using RedDeer.Extensions.Security.Authentication.Jwt;
using RedDeer.Extensions.Security.Authentication.Jwt.Abstractions;
using StructureMap;
using Surveillance.Data.Universe.Refinitiv.Interfaces;

namespace Surveillance.Data.Universe.Refinitiv
{
    public class RefinitivRegistry : Registry
    {
        public RefinitivRegistry()
        {
            this.For<ITickPriceHistoryServiceClientFactory>().Use<TickPriceHistoryServiceClientFactory>();
            this.For<IRefinitivTickPriceHistoryApi>().Use<RefinitivTickPriceHistoryApi>();
            this.For<IJwtTokenService>().Use<JwtTokenService>();
        }
    }
}
