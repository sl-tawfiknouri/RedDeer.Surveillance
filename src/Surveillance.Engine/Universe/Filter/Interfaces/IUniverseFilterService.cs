namespace Surveillance.Engine.Rules.Universe.Filter.Interfaces
{
    using System;

    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    public interface IUniverseFilterService : IObservable<IUniverseEvent>, IUniverseRule
    {
    }
}