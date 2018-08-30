using System;

namespace Surveillance.Factories
{
    public class OriginFactory : IOriginFactory
    {
        public string Origin()
        {
            return $"{Environment.MachineName}:surveillance-service";
        }
    }
}
