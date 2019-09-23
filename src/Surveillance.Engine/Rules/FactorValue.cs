namespace Surveillance.Engine.Rules.Rules
{
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    public class FactorValue : IFactorValue
    {
        public FactorValue(ClientOrganisationalFactors organisationalFactors, string value)
        {
            this.OrganisationalFactors = organisationalFactors;
            this.Value = value ?? string.Empty;
        }

        public static IFactorValue None => new FactorValue(ClientOrganisationalFactors.None, string.Empty);

        public ClientOrganisationalFactors OrganisationalFactors { get; }

        public string Value { get; }
    }
}