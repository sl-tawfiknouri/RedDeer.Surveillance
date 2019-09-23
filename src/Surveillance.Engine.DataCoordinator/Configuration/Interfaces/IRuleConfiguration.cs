namespace Surveillance.Engine.DataCoordinator.Configuration.Interfaces
{
    public interface IRuleConfiguration
    {
        bool? AlwaysRequireAllocations { get; set; }

        /// <summary>
        ///     Auto schedule rules
        /// </summary>
        bool? AutoScheduleRules { get; set; }
    }
}