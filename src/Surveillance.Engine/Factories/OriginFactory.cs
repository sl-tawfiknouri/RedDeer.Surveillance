namespace Surveillance.Engine.Rules.Factories
{
    using System;

    using Surveillance.Engine.Rules.Factories.Interfaces;

    public class OriginFactory : IOriginFactory
    {
        public string Origin()
        {
            return $"{Environment.MachineName}:surveillance-service";
        }
    }
}