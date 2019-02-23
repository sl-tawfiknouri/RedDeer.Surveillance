namespace Surveillance.Engine.Rules.Rules.Interfaces
{
    public interface IUniverseCloneableRule : IUniverseRule
    {
        IUniverseCloneableRule Clone(IFactorValue factor);
        IFactorValue OrganisationFactorValue { get; set; }
    }
}
