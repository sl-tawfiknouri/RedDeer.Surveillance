using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Rules
{
    public class FactorValue : IFactorValue
    {
        public FactorValue(
            ClientOrganisationalFactors organisationalFactors,
            string value)
        {
            OrganisationalFactors = organisationalFactors;
            Value = value ?? string.Empty;
        }

        public ClientOrganisationalFactors OrganisationalFactors { get; }
        public string Value { get; }

        public static IFactorValue None => new FactorValue(ClientOrganisationalFactors.None, string.Empty);
    }
}
