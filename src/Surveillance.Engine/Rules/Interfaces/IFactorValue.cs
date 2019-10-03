namespace Surveillance.Engine.Rules.Rules.Interfaces
{
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;

    /// <summary>
    /// The FactorValue interface.
    /// </summary>
    public interface IFactorValue
    {
        /// <summary>
        /// Gets the organizational factors.
        /// </summary>
        ClientOrganisationalFactors OrganisationalFactors { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        string Value { get; }
    }
}