using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Equity.Frames;
using DomainV2.Financial;
using DomainV2.Trading;
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
        private readonly IDictionary<InstrumentIdentifiers, FinancialInstrument> _securityIds;

        private readonly object _traderIdLock = new object();
        private readonly object _marketIdLock = new object();
        private readonly object _securityIdLock = new object();

        public UniverseAnalyticsSubscriber(int operationContextId)
        {
            Analytics = new UniverseAnalytics {SystemProcessOperationId = operationContextId};
            _traderIds = new Dictionary<string, string>();
            _marketIds = new Dictionary<string, string>();
            _securityIds = new Dictionary<InstrumentIdentifiers, FinancialInstrument>();
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
                            _securityIds.Add(new KeyValuePair<InstrumentIdentifiers, FinancialInstrument>(sec.Security.Identifiers, sec.Security));
                        }
                    }
                }
            }
        }

        private void TradeUpdate(IUniverseEvent value)
        {
            Analytics.TradeReddeerCount += 1;

            var tradeFrame = (Order)value.UnderlyingEvent;

            if (tradeFrame.Trades == null
                || !tradeFrame.Trades.Any())
            {
                return;
            }

            lock (_traderIdLock)
            {
                foreach (var trade in tradeFrame.Trades.Where(tf => tf != null).ToList())

                if (!_traderIds.ContainsKey(trade.TraderId.ToLower()))
                {
                    _traderIds.Add(new KeyValuePair<string, string>(trade.TraderId.ToLower(), trade.TraderId.ToLower()));
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