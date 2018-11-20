using System;
using Surveillance.Analytics.Streams.Interfaces;

namespace Surveillance.Analytics.Subscriber.Interfaces
{
    public interface IUniverseAlertSubscriber : IObserver<IUniverseAlertEvent>
    { }
}
