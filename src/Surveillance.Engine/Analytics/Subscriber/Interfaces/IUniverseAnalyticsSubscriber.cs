namespace Surveillance.Engine.Rules.Analytics.Subscriber.Interfaces
{
    using System;

    using Surveillance.DataLayer.Aurora.Analytics;
    using Surveillance.Engine.Rules.Universe.Interfaces;

    public interface IUniverseAnalyticsSubscriber : IObserver<IUniverseEvent>
    {
        UniverseAnalytics Analytics { get; }
    }
}