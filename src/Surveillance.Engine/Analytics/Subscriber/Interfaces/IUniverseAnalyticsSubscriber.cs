namespace Surveillance.Engine.Rules.Analytics.Subscriber.Interfaces
{
    using System;

    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.DataLayer.Aurora.Analytics;

    public interface IUniverseAnalyticsSubscriber : IObserver<IUniverseEvent>
    {
        UniverseAnalytics Analytics { get; }
    }
}