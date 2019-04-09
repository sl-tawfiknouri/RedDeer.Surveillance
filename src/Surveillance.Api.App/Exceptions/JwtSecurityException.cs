using System;

namespace Surveillance.Api.App.Exceptions
{
    public class JwtMissingSecurityException : Exception
    {
        public override string Message => "JWT configuration setting for secret key was not set";
    }
}
