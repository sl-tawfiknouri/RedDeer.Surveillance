namespace Surveillance.Engine.Rules.Data.Subscribers.Interfaces
{
    using System;

    using Surveillance.Engine.Rules.Universe.Interfaces;

    public interface IUniverseDataRequestsSubscriber : IObserver<IUniverseEvent>
    {
        bool SubmitRequests { get; }

        void DispatchIfSubmitRequest();

        void SubmitRequest();
    }
}