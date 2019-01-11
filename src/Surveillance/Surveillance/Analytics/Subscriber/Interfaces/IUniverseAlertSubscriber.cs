using System;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.DataLayer.Aurora.Analytics;

namespace Surveillance.Analytics.Subscriber.Interfaces
{
    public interface IUniverseAlertSubscriber : IObserver<IUniverseAlertEvent>
    {
        AlertAnalytics Analytics { get; }
        void Flush();
    }
}
