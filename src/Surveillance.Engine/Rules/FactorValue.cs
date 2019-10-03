namespace Surveillance.Engine.Rules.Rules
{
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    /// <summary>
    /// The factor value.
    /// </summary>
    public class FactorValue : IFactorValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FactorValue"/> class.
        /// </summary>
        /// <param name="organisationalFactors">
        /// The organizational factors.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public FactorValue(ClientOrganisationalFactors organisationalFactors, string value)
        {
            this.OrganisationalFactors = organisationalFactors;
            this.Value = value ?? string.Empty;
        }

        /// <summary>
        /// The none.
        /// </summary>
        public static IFactorValue None => new FactorValue(ClientOrganisationalFactors.None, string.Empty);

        /// <summary>
        /// Gets the organizational factors.
        /// </summary>
        public ClientOrganisationalFactors OrganisationalFactors { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public string Value { get; }
    }
}