namespace Surveillance.Engine.Rules.Analytics.Subscriber
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Markets.Collections;
    using Domain.Core.Trading.Orders;

    using Microsoft.Extensions.Logging;

    using Surveillance.Data.Universe;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Data.Universe.MarketEvents;
    using Surveillance.DataLayer.Aurora.Analytics;
    using Surveillance.Engine.Rules.Analytics.Subscriber.Interfaces;

    public class UniverseAnalyticsSubscriber : IUniverseAnalyticsSubscriber
    {
        private readonly ILogger<UniverseAnalyticsSubscriber> _logger;

        private readonly object _marketIdLock = new object();

        private readonly IDictionary<string, string> _marketIds;

        private readonly object _securityIdLock = new object();

        private readonly IDictionary<InstrumentIdentifiers, FinancialInstrument> _securityIds;

        private readonly object _traderIdLock = new object();

        private readonly IDictionary<string, string> _traderIds;

        public UniverseAnalyticsSubscriber(int operationContextId, ILogger<UniverseAnalyticsSubscriber> logger)
        {
            this.Analytics = new UniverseAnalytics { SystemProcessOperationId = operationContextId };
            this._traderIds = new Dictionary<string, string>();
            this._marketIds = new Dictionary<string, string>();
            this._securityIds = new Dictionary<InstrumentIdentifiers, FinancialInstrument>();
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public UniverseAnalytics Analytics { get; }

        public void OnCompleted()
        {
            this._logger.LogInformation("received the OnCompleted() event from the analytics stream");
        }

        public void OnError(Exception error)
        {
            this._logger.LogError("received the OnError() event from the analytics stream", error);
        }

        public void OnNext(IUniverseEvent value)
        {
            if (value == null)
            {
                this._logger.LogWarning("received a null analytics stream event.");
                return;
            }

            this._logger.LogInformation(
                $"received an analytics event of type {value.StateChange} at {value.EventTime} universe time.");

            switch (value.StateChange)
            {
                case UniverseStateEvent.Genesis:
                    this.Analytics.GenesisEventCount += 1;
                    break;
                case UniverseStateEvent.Eschaton:
                    this.Eschaton();
                    break;
                case UniverseStateEvent.ExchangeClose:
                    this.Analytics.StockMarketCloseCount += 1;
                    break;
                case UniverseStateEvent.ExchangeOpen:
                    this.MarketOpen(value);
                    break;
                case UniverseStateEvent.EquityIntradayTick:
                    this.StockTick(value);
                    break;
                case UniverseStateEvent.Order:
                    this.TradeUpdate(value);
                    break;
                case UniverseStateEvent.OrderPlaced:
                    this.Analytics.TradeReddeerSubmittedCount += 1;
                    break;
            }
        }

        private void Eschaton()
        {
            this.Analytics.EschatonEventCount += 1;
            this.Analytics.UniqueTradersCount = this._traderIds.Count;
            this.Analytics.UniqueMarketsTradedOnCount = this._marketIds.Count;
            this.Analytics.UniqueSecuritiesCount = this._securityIds.Count;

            this._logger.LogInformation(
                $"received eschaton event considering there to be {this.Analytics.EschatonEventCount} eschaton events; {this.Analytics.UniqueTradersCount} unique traders; {this.Analytics.UniqueMarketsTradedOnCount} unique markets traded on; and {this.Analytics.UniqueSecuritiesCount} unique securities");
        }

        private void MarketOpen(IUniverseEvent value)
        {
            this.Analytics.StockMarketOpenCount += 1;
            var marketFrame = (MarketOpenClose)value.UnderlyingEvent;

            if (string.IsNullOrWhiteSpace(marketFrame?.MarketId)) return;

            lock (this._marketIdLock)
            {
                if (!this._marketIds.ContainsKey(marketFrame.MarketId?.ToLower()))
                    this._marketIds.Add(
                        new KeyValuePair<string, string>(marketFrame.MarketId.ToLower(), marketFrame.MarketId));
            }
        }

        private void StockTick(IUniverseEvent value)
        {
            this.Analytics.StockTickReddeerCount += 1;

            var exchangeFrame = (EquityIntraDayTimeBarCollection)value.UnderlyingEvent;

            if (exchangeFrame?.Securities == null) return;

            lock (this._securityIdLock)
            {
                if (exchangeFrame?.Securities?.Count != 0)
                    foreach (var sec in exchangeFrame.Securities)
                    {
                        if (sec?.Security?.Identifiers == null) continue;

                        if (!this._securityIds.ContainsKey(sec.Security.Identifiers))
                            this._securityIds.Add(
                                new KeyValuePair<InstrumentIdentifiers, FinancialInstrument>(
                                    sec.Security.Identifiers,
                                    sec.Security));
                    }
            }
        }

        private void TradeUpdate(IUniverseEvent value)
        {
            this.Analytics.TradeReddeerCount += 1;

            var tradeFrame = (Order)value.UnderlyingEvent;

            if (tradeFrame.DealerOrders == null || !tradeFrame.DealerOrders.Any()) return;

            lock (this._traderIdLock)
            {
                foreach (var trade in tradeFrame.DealerOrders.Where(tf => tf != null).ToList())

                    if (!this._traderIds.ContainsKey(trade.DealerId.ToLower()))
                        this._traderIds.Add(
                            new KeyValuePair<string, string>(trade.DealerId.ToLower(), trade.DealerId.ToLower()));
            }
        }
    }
}