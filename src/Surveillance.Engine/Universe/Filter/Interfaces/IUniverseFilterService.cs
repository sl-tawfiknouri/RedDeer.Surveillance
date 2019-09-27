namespace Surveillance.Engine.Rules.Universe.Filter.Interfaces
{
    using System;

    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    /// <summary>
    /// The UniverseFilterService interface.
    /// </summary>
    public interface IUniverseFilterService : IObservable<IUniverseEvent>, IUniverseRule
    {
    }
}