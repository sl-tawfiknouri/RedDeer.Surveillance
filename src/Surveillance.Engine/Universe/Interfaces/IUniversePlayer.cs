using System;

namespace Surveillance.Engine.Rules.Universe.Interfaces
{
    public interface IUniversePlayer : IObservable<IUniverseEvent>
    {
        void Play(IUniverse universe);
    }
}
