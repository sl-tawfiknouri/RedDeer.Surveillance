using System;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Analytics.Subscriber.Interfaces
{
    public interface IUniverseAnalyticsSubscriber : IObserver<IUniverseEvent>
    { }
}
