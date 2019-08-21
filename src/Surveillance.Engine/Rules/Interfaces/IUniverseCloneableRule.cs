namespace Surveillance.Engine.Rules.Rules.Interfaces
{
    using System;

    public interface IUniverseCloneableRule : IUniverseRule, ICloneable
    {
        IFactorValue OrganisationFactorValue { get; set; }

        IUniverseCloneableRule Clone(IFactorValue factor);
    }
}