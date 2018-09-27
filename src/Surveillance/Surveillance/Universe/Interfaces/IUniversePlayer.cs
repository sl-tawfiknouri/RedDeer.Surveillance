using System;

namespace Surveillance.Universe.Interfaces
{
    public interface IUniversePlayer : IObservable<IUniverseEvent>
    {
        void Play(IUniverse universe);
    }
}
