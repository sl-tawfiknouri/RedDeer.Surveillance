using System;
using System.Security.Claims;
using Security.Core.Abstractions;
using Surveillance.Api.App.Infrastructure.Interfaces;

namespace Surveillance.Api.App.Infrastructure
{
    public class SurveillanceAuthorisation : ISurveillanceAuthorisation
    {
        public SurveillanceAuthorisation(IClaimsManifest manifest)
        {
            Manifest = manifest;
        }

        public IClaimsManifest Manifest { get; }

        public bool IsAuthorised(ClaimsPrincipal principal)
        {
            return principal.HasClaim(i =>
                string.Equals(i.Value, Manifest.SurveillanceReaderPrivilege, StringComparison.OrdinalIgnoreCase)
                || string.Equals(i.Value, Manifest.SurveillanceReader, StringComparison.OrdinalIgnoreCase));
        }
    }
}
