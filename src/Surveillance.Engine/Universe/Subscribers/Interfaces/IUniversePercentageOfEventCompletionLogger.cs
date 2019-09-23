namespace Surveillance.Engine.Rules.Universe.Subscribers.Interfaces
{
    using System;

    using Surveillance.Engine.Rules.Universe.Interfaces;

    public interface IUniversePercentageOfEventCompletionLogger : IObserver<IUniverseEvent>
    {
        void InitiateEventLogger(IUniverse universe);
    }
}