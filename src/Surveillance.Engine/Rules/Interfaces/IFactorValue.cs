using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;

namespace Surveillance.Engine.Rules.Rules.Interfaces
{
    public interface IFactorValue
    {
        ClientOrganisationalFactors OrganisationalFactors { get; }
        string Value { get; }
    }
}