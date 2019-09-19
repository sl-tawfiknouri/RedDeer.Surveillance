namespace Surveillance.Engine.DataCoordinator.Configuration.Interfaces
{
    /// <summary>
    /// The RuleConfiguration interface.
    /// </summary>
    public interface IRuleConfiguration
    {
        /// <summary>
        /// Gets or sets the always require allocations.
        /// </summary>
        bool? AlwaysRequireAllocations { get; set; }

        /// <summary>
        ///     Auto schedule rules
        /// </summary>
        bool? AutoScheduleRules { get; set; }
    }
}