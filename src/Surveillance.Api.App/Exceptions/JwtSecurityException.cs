namespace Surveillance.Api.App.Exceptions
{
    using System;

    public class JwtMissingSecurityException : Exception
    {
        public override string Message => "JWT configuration setting for secret key was not set";
    }
}