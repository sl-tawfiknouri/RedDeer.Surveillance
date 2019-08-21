namespace Surveillance.Engine.Rules.Universe.Interfaces
{
    using System;

    public interface IUniversePlayer : IObservable<IUniverseEvent>
    {
        void Play(IUniverse universe);
    }
}