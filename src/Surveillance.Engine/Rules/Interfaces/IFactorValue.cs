namespace Surveillance.Engine.Rules.Rules.Interfaces
{
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;

    public interface IFactorValue
    {
        ClientOrganisationalFactors OrganisationalFactors { get; }

        string Value { get; }
    }
}