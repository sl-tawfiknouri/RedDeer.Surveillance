using Surveillance.Configuration.Interfaces;

namespace Surveillance.Configuration
{
    public class RuleConfiguration : IRuleConfiguration
    {
        public bool? AutoScheduleRules { get; set; }
    }
}
