using System;
using System.Collections.Concurrent;
using System.Linq;
using DomainV2.Equity.Frames;
using DomainV2.Equity.Streams.Interfaces;
using DomainV2.Trading;
using Surveillance.RuleParameters.Filter;
using Surveillance.Rules;
using Surveillance.Universe.Filter.Interfaces;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe.Filter
{
    public class UniverseFilter : IUniverseFilter
    {
        private readonly IUnsubscriberFactory<IUniverseEvent> _universeUnsubscriberFactory;
        private readonly ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>> _universeObservers;

        private readonly RuleFilter _accounts;
        private readonly RuleFilter _traders;
        private readonly RuleFilter _markets;

        public UniverseFilter(
            IUnsubscriberFactory<IUniverseEvent> universeUnsubscriberFactory,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets)
        {
            _universeUnsubscriberFactory =
                universeUnsubscriberFactory
                ?? throw new ArgumentNullException(nameof(universeUnsubscriberFactory));

            _accounts = accounts;
            _traders = traders;
            _markets = markets;

            _universeObservers = new ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>>();
        }

        public IDisposable Subscribe(IObserver<IUniverseEvent> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            if (!_universeObservers.ContainsKey(observer))
            {
                _universeObservers.TryAdd(observer, observer);
            }

            return _universeUnsubscriberFactory.Create(_universeObservers, observer);
        }

        public void OnCompleted()
        {
            foreach (var obs in _universeObservers)
            {
                obs.Value?.OnCompleted();
            }
        }

        public void OnError(Exception error)
        {
            foreach (var obs in _universeObservers)
            {
                obs.Value?.OnError(error);
            }
        }

        public void OnNext(IUniverseEvent value)
        {
            if (FilterOnAccount(value))
            {
                return;
            }

            if (FilterOnTraders(value))
            {
                return;
            }

            if (FilterOnMarkets(value))
            {
                return;
            }

            foreach (var obs in _universeObservers)
            {
                obs.Value?.OnNext(value);
            }
        }

        private bool FilterOnAccount(IUniverseEvent value)
        {
            if (_accounts == null)
            {
                return false;
            }

            if (_accounts.Type == RuleFilterType.None)
            {
                return false;
            }

            if (value == null)
            {
                return false;
            }

            if (value.StateChange != UniverseStateEvent.TradeReddeer
                && value.StateChange != UniverseStateEvent.TradeReddeerSubmitted)
            {
                return false;
            }

            var frame = (Order)value.UnderlyingEvent;

            if (frame == null)
            {
                return false;
            }

            switch (_accounts.Type)
            {
                case RuleFilterType.Include:
                    return !_accounts.Ids.Contains(frame.OrderClientAccountAttributionId, StringComparer.InvariantCultureIgnoreCase);
                case RuleFilterType.Exclude:
                    return _accounts.Ids.Contains(frame.OrderClientAccountAttributionId, StringComparer.InvariantCultureIgnoreCase);
                default:
                    return false;
            }
        }

        private bool FilterOnTraders(IUniverseEvent value)
        {
            if (_traders == null)
            {
                return false;
            }

            if (_traders.Type == RuleFilterType.None)
            {
                return false;
            }

            if (value == null)
            {
                return false;
            }

            if (value.StateChange != UniverseStateEvent.TradeReddeer
                && value.StateChange != UniverseStateEvent.TradeReddeerSubmitted)
            {
                return false;
            }

            var frame = (Order)value.UnderlyingEvent;

            if (frame == null)
            {
                return false;
            }

            switch (_traders.Type)
            {
                case RuleFilterType.Include:
                    return !_traders.Ids.Contains(frame.OrderTraderId, StringComparer.InvariantCultureIgnoreCase);
                case RuleFilterType.Exclude:
                    return _traders.Ids.Contains(frame.OrderTraderId, StringComparer.InvariantCultureIgnoreCase);
                default:
                    return false;
            }
        }

        private bool FilterOnMarkets(IUniverseEvent value)
        {
            if (_markets == null)
            {
                return false;
            }

            if (_markets.Type == RuleFilterType.None)
            {
                return false;
            }

            if (value == null)
            {
                return false;
            }

            if (value.StateChange != UniverseStateEvent.TradeReddeer
                && value.StateChange != UniverseStateEvent.TradeReddeerSubmitted
                && value.StateChange != UniverseStateEvent.StockTickReddeer)
            {
                return false;
            }

            if (value.StateChange == UniverseStateEvent.StockTickReddeer)
            {
                var exchFrame = (ExchangeFrame)value.UnderlyingEvent;
                if (exchFrame == null)
                {
                    return false;
                }

                if (_markets.Type == RuleFilterType.Include)
                {
                    if (exchFrame?.Exchange?.MarketIdentifierCode == null)
                    {
                        return true;
                    }

                    return !_markets.Ids.Contains(exchFrame.Exchange.MarketIdentifierCode, StringComparer.InvariantCultureIgnoreCase);
                }

                if (_markets.Type == RuleFilterType.Exclude)
                {
                    if (exchFrame?.Exchange?.MarketIdentifierCode == null)
                    {
                        return false;
                    }

                    return _markets.Ids.Contains(exchFrame.Exchange.MarketIdentifierCode, StringComparer.InvariantCultureIgnoreCase);
                }

                return false;
            }

            if (value.StateChange == UniverseStateEvent.TradeReddeer
                || value.StateChange == UniverseStateEvent.TradeReddeerSubmitted)
            {
                var tradeFrame = (Order)value.UnderlyingEvent;
                if (tradeFrame == null)
                {
                    return false;
                }

                if (_markets.Type == RuleFilterType.Include)
                {
                    if (tradeFrame?.Market?.MarketIdentifierCode == null)
                    {
                        return false;
                    }

                    return !_markets.Ids.Contains(tradeFrame.Market.MarketIdentifierCode, StringComparer.InvariantCultureIgnoreCase);
                }

                if (_markets.Type == RuleFilterType.Exclude)
                {
                    if (tradeFrame?.Market?.MarketIdentifierCode == null)
                    {
                        return true;
                    }

                    return _markets.Ids.Contains(tradeFrame.Market.MarketIdentifierCode, StringComparer.InvariantCultureIgnoreCase);
                }

                return false;
            }

            return false;
        }

        public DomainV2.Scheduling.Rules Rule { get; } = DomainV2.Scheduling.Rules.UniverseFilter;
        public string Version { get; } = Versioner.Version(0, 0);

        public object Clone()
        {
            // we will want to keep the same universe observers here
            return this.MemberwiseClone();
        }
    }
}