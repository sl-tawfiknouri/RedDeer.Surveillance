namespace Surveillance.Data.Universe.Interfaces
{
    using System;

    public interface IUniversePlayer : IObservable<IUniverseEvent>
    {
        void Play(IUniverse universe);
    }
}