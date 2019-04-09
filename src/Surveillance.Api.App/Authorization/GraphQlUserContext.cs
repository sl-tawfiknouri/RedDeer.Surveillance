using System.Security.Claims;
using GraphQL.Authorization;

namespace Surveillance.Api.App.Authorization
{
    public class GraphQlUserContext : IProvideClaimsPrincipal
    {
        public ClaimsPrincipal User { get; set; }
    }
}
