using System;
using Surveillance.Analytics.Subscriber.Interfaces;
using Surveillance.Universe;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Analytics.Subscriber
{
    public class UniverseAnalyticsSubscriber : IUniverseAnalyticsSubscriber
    {    
        public UniverseAnalyticsSubscriber()
        {
            Analytics = new UniverseAnalytics();
        }

        public void OnCompleted()
        {}

        public void OnError(Exception error)
        {}

        public void OnNext(IUniverseEvent value)
        {
            if (value == null)
            {
                return;
            }

            switch (value.StateChange)
            {
                case UniverseStateEvent.Genesis:
                    Analytics.GenesisEventCount += 1;
                    break;
                case UniverseStateEvent.Eschaton:
                    Analytics.EschatonEventCount += 1;
                    break;
                case UniverseStateEvent.StockMarketClose:
                    Analytics.StockMarketCloseCount += 1;
                    break;
                case UniverseStateEvent.StockMarketOpen:
                    Analytics.StockMarketOpenCount += 1;
                    break;
                case UniverseStateEvent.StockTickReddeer:
                    Analytics.StockTickReddeerCount += 1;
                    break;
                case UniverseStateEvent.TradeReddeer:
                    Analytics.TradeReddeerCount += 1;
                    break;
                case UniverseStateEvent.TradeReddeerSubmitted:
                    Analytics.TradeReddeerSubmittedCount += 1;
                    break;
                case UniverseStateEvent.Unknown:
                    Analytics.UnknownEventCount += 1;
                    break;
            }
        }

        public UniverseAnalytics Analytics { get; }
    }
}
