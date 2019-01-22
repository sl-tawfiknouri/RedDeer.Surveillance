using System;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Data.Subscribers.Interfaces
{
    public interface IUniverseDataRequestsSubscriber : IObserver<IUniverseEvent>
    {
        void SubmitRequest();
    }
}
