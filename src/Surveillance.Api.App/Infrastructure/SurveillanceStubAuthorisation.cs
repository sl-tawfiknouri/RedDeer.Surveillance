using Surveillance.Api.App.Infrastructure.Interfaces;
using System.Security.Claims;

namespace Surveillance.Api.App.Infrastructure
{
    public class SurveillanceStubAuthorisation : ISurveillanceAuthorisation
    {
        public bool CanReadApi(ClaimsPrincipal principal)
        {
            return true;
        }
    }
}
