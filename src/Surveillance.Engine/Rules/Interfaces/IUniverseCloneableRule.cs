using System;

namespace Surveillance.Engine.Rules.Rules.Interfaces
{
    public interface IUniverseCloneableRule : IUniverseRule, ICloneable
    {
        IUniverseCloneableRule Clone(IFactorValue factor);
        IFactorValue OrganisationFactorValue { get; set; }
    }
}
