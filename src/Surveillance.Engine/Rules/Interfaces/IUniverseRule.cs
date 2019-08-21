namespace Surveillance.Engine.Rules.Rules.Interfaces
{
    using System;

    using Domain.Surveillance.Scheduling;

    using Surveillance.Engine.Rules.Universe.Interfaces;

    public interface IUniverseRule : IObserver<IUniverseEvent>
    {
        Rules Rule { get; }

        string Version { get; }
    }
}