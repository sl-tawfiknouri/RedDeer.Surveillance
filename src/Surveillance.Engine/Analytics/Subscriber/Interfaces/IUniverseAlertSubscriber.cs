using System;
using Surveillance.DataLayer.Aurora.Analytics;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;

namespace Surveillance.Engine.Rules.Analytics.Subscriber.Interfaces
{
    public interface IUniverseAlertSubscriber : IObserver<IUniverseAlertEvent>
    {
        AlertAnalytics Analytics { get; }
        void Flush();
    }
}
