using Surveillance.Engine.Rules.Configuration.Interfaces;

namespace Surveillance.Engine.Rules.Configuration
{
    public class RuleConfiguration : IRuleConfiguration
    {
        public bool? AutoScheduleRules { get; set; }
    }
}
