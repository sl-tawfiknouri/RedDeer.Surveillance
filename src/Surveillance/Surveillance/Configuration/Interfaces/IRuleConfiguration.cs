namespace Surveillance.Configuration.Interfaces
{
    public interface IRuleConfiguration
    {
        /// <summary>
        /// Auto schedule rules
        /// </summary>
        bool? AutoScheduleRules { get; set; }
    }
}
