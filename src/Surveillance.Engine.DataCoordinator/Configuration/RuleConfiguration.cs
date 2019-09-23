namespace Surveillance.Engine.DataCoordinator.Configuration
{
    using Surveillance.Engine.DataCoordinator.Configuration.Interfaces;

    public class RuleConfiguration : IRuleConfiguration
    {
        public bool? AlwaysRequireAllocations { get; set; }

        public bool? AutoScheduleRules { get; set; }
    }
}