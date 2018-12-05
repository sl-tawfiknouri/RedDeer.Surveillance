using System;
using System.Collections.Generic;
using Domain.Trades.Orders;
using DomainV2.Equity;
using DomainV2.Equity.Frames;
using Surveillance.Analytics.Subscriber.Interfaces;
using Surveillance.DataLayer.Aurora.Analytics;
using Surveillance.Universe;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.MarketEvents;

namespace Surveillance.Analytics.Subscriber
{
    public class UniverseAnalyticsSubscriber : IUniverseAnalyticsSubscriber
    {
        private readonly IDictionary<string, string> _traderIds;
        private readonly IDictionary<string, string> _marketIds;
        private readonly IDictionary<SecurityIdentifiers, Security> _securityIds;

        private readonly object _traderIdLock = new object();
        private readonly object _marketIdLock = new object();
        private readonly object _securityIdLock = new object();

        public UniverseAnalyticsSubscriber(int operationContextId)
        {
            Analytics = new UniverseAnalytics {SystemProcessOperationId = operationContextId};
            _traderIds = new Dictionary<string, string>();
            _marketIds = new Dictionary<string, string>();
            _securityIds = new Dictionary<SecurityIdentifiers, Security>();
        }

        public UniverseAnalytics Analytics { get; }

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
                    Eschaton();
                    break;
                case UniverseStateEvent.StockMarketClose:
                    Analytics.StockMarketCloseCount += 1;
                    break;
                case UniverseStateEvent.StockMarketOpen:
                    MarketOpen(value);
                    break;
                case UniverseStateEvent.StockTickReddeer:
                    StockTick(value);
                    break;
                case UniverseStateEvent.TradeReddeer:
                    TradeUpdate(value);
                    break;
                case UniverseStateEvent.TradeReddeerSubmitted:
                    Analytics.TradeReddeerSubmittedCount += 1;
                    break;
                case UniverseStateEvent.Unknown:
                    Analytics.UnknownEventCount += 1;
                    break;
            }
        }

        private void MarketOpen(IUniverseEvent value)
        {
            Analytics.StockMarketOpenCount += 1;
            var marketFrame = (MarketOpenClose)value.UnderlyingEvent;

            if (string.IsNullOrWhiteSpace(marketFrame?.MarketId))
            {
                return;
            }

            lock (_marketIdLock)
            {
                if (!_marketIds.ContainsKey(marketFrame.MarketId?.ToLower()))
                {
                    _marketIds.Add(new KeyValuePair<string, string>(marketFrame.MarketId.ToLower(), marketFrame.MarketId));
                }
            }
        }

        private void StockTick(IUniverseEvent value)
        {
            Analytics.StockTickReddeerCount += 1;

            var exchangeFrame = (ExchangeFrame) value.UnderlyingEvent;

            if (exchangeFrame?.Securities == null)
            {
                return;
            }

            lock (_securityIdLock)
            {
                if (exchangeFrame?.Securities?.Count != 0)
                {
                    foreach (var sec in exchangeFrame.Securities)
                    {
                        if (sec?.Security?.Identifiers == null)
                        {
                            continue;
                        }

                        if (!_securityIds.ContainsKey(sec.Security.Identifiers))
                        {
                            _securityIds.Add(new KeyValuePair<SecurityIdentifiers, Security>(sec.Security.Identifiers, sec.Security));
                        }
                    }
                }
            }
        }

        private void TradeUpdate(IUniverseEvent value)
        {
            Analytics.TradeReddeerCount += 1;

            var tradeFrame = (TradeOrderFrame)value.UnderlyingEvent;

            if (string.IsNullOrWhiteSpace(tradeFrame?.TraderId))
            {
                return;
            }

            lock (_traderIdLock)
            {
                if (!_traderIds.ContainsKey(tradeFrame.TraderId?.ToLower()))
                {
                    _traderIds.Add(new KeyValuePair<string, string>(tradeFrame.TraderId.ToLower(), tradeFrame.TraderId));
                }
            }
        }

        private void Eschaton()
        {
            Analytics.EschatonEventCount += 1;

            Analytics.UniqueTradersCount = _traderIds.Count;
            Analytics.UniqueMarketsTradedOnCount = _marketIds.Count;
            Analytics.UniqueSecuritiesCount = _securityIds.Count;
        }
    }
}