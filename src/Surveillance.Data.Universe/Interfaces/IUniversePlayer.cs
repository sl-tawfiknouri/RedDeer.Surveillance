namespace Surveillance.Data.Universe.Interfaces
{
    using System;

    /// <summary>
    /// The UniversePlayer interface.
    /// </summary>
    public interface IUniversePlayer : IObservable<IUniverseEvent>
    {
        /// <summary>
        /// The play.
        /// </summary>
        /// <param name="universe">
        /// The universe.
        /// </param>
        void Play(IUniverse universe);
    }
}