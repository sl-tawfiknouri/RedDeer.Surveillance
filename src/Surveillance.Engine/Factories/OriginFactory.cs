using System;
using Surveillance.Engine.Rules.Factories.Interfaces;

namespace Surveillance.Engine.Rules.Factories
{
    public class OriginFactory : IOriginFactory
    {
        public string Origin()
        {
            return $"{Environment.MachineName}:surveillance-service";
        }
    }
}
