using System;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Data.Subscribers.Interfaces
{
    public interface IUniverseDataRequestsSubscriber : IObserver<IUniverseEvent>
    {
        void SubmitRequest();
        bool SubmitRequests { get; }
    }
}
