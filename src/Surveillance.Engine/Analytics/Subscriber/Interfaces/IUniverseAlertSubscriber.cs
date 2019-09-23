namespace Surveillance.Engine.Rules.Analytics.Subscriber.Interfaces
{
    using System;

    using Surveillance.DataLayer.Aurora.Analytics;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;

    public interface IUniverseAlertSubscriber : IObserver<IUniverseAlertEvent>
    {
        AlertAnalytics Analytics { get; }

        void Flush();
    }
}