using Surveillance.Engine.DataCoordinator.Configuration.Interfaces;

namespace Surveillance.Engine.DataCoordinator.Configuration
{
    public class RuleConfiguration : IRuleConfiguration
    {
        public bool? AutoScheduleRules { get; set; }
        public bool? AlwaysRequireAllocations { get; set; }
    }
}
