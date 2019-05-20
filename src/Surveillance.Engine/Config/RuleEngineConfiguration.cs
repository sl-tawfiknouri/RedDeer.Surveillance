using System;
using Surveillance.Engine.Rules.Config.Interfaces;

namespace Surveillance.Engine.Rules.Config
{
    public class RuleEngineConfiguration : IRuleEngineConfiguration
    {
        public RuleEngineConfiguration(string client, string environment)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public string Client { get; }
        public string Environment { get; }
    }
}
