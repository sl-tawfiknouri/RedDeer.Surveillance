namespace Surveillance.Engine.DataCoordinator.Configuration
{
    using Surveillance.Engine.DataCoordinator.Configuration.Interfaces;

    /// <summary>
    /// The rule configuration.
    /// </summary>
    public class RuleConfiguration : IRuleConfiguration
    {
        /// <summary>
        /// Gets or sets the always require allocations.
        /// </summary>
        public bool? AlwaysRequireAllocations { get; set; }

        /// <summary>
        /// Gets or sets the auto schedule rules.
        /// </summary>
        public bool? AutoScheduleRules { get; set; }
    }
}