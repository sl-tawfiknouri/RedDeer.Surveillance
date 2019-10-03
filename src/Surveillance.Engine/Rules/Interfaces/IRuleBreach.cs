namespace Surveillance.Engine.Rules.Rules.Interfaces
{
    /// <summary>
    /// The RuleBreach interface.
    /// </summary>
    public interface IRuleBreach : IRuleBreachContext
    {
        /// <summary>
        /// Gets or sets the case title.
        /// </summary>
        string CaseTitle { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        string Description { get; set; }
    }
}