using System.Security.Claims;

namespace Surveillance.Api.App.Infrastructure.Interfaces
{
    public interface ISurveillanceAuthorisation
    {
        bool IsAuthorised(ClaimsPrincipal principal);
    }
}