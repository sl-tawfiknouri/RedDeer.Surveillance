namespace Surveillance.Engine.Rules.Universe.Subscribers.Interfaces
{
    using System;

    using Surveillance.Data.Universe.Interfaces;

    /// <summary>
    /// The UniversePercentageOfEventCompletionLogger interface.
    /// </summary>
    public interface IUniversePercentageOfEventCompletionLogger : IObserver<IUniverseEvent>
    {
        /// <summary>
        /// The initiate event logger.
        /// </summary>
        /// <param name="universe">
        /// The universe.
        /// </param>
        void InitiateEventLogger(IUniverse universe);
    }
}