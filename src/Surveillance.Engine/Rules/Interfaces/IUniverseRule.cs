using System;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Interfaces
{
    public interface IUniverseRule : IObserver<IUniverseEvent>
    {
        DomainV2.Scheduling.Rules Rule { get; }
        string Version { get; }
    }
}
