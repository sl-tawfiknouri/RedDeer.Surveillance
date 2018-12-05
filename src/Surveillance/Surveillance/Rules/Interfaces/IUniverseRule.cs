using System;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Rules.Interfaces
{
    public interface IUniverseRule : IObserver<IUniverseEvent>
    {
        DomainV2.Scheduling.Rules Rule { get; }
        string Version { get; }
    }
}
