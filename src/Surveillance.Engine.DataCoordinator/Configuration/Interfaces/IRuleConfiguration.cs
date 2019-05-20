namespace Surveillance.Engine.DataCoordinator.Configuration.Interfaces
{
    public interface IRuleConfiguration
    {
        /// <summary>
        /// Auto schedule rules
        /// </summary>
        bool? AutoScheduleRules { get; set; }

        bool? AlwaysRequireAllocations { get; set; }
    }
}
