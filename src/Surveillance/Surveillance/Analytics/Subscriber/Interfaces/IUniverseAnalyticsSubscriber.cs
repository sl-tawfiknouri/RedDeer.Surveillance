using System;
using Surveillance.DataLayer.Aurora.Analytics;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Analytics.Subscriber.Interfaces
{
    public interface IUniverseAnalyticsSubscriber : IObserver<IUniverseEvent>
    {
        UniverseAnalytics Analytics { get; }
    }
}
