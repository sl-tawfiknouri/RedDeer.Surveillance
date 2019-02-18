using System;
using Surveillance.DataLayer.Aurora.Analytics;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Analytics.Subscriber.Interfaces
{
    public interface IUniverseAnalyticsSubscriber : IObserver<IUniverseEvent>
    {
        UniverseAnalytics Analytics { get; }
    }
}
