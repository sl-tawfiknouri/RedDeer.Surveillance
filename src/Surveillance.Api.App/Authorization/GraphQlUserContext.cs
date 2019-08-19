namespace Surveillance.Api.App.Authorization
{
    using System.Security.Claims;

    using GraphQL.Authorization;

    public class GraphQlUserContext : IProvideClaimsPrincipal
    {
        public ClaimsPrincipal User { get; set; }
    }
}