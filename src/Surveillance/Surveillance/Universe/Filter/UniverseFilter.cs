using System;
using System.Collections.Concurrent;
using System.Linq;
using DomainV2.Equity.Streams.Interfaces;
using DomainV2.Equity.TimeBars;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<UniverseFilter> _logger;

        public UniverseFilter(
            IUnsubscriberFactory<IUniverseEvent> universeUnsubscriberFactory,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets,
            ILogger<UniverseFilter> logger)
        {
            _universeUnsubscriberFactory =
                universeUnsubscriberFactory
                ?? throw new ArgumentNullException(nameof(universeUnsubscriberFactory));

            _accounts = accounts;
            _traders = traders;
            _markets = markets;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _universeObservers = new ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>>();
        }

        public IDisposable Subscribe(IObserver<IUniverseEvent> observer)
        {
            if (observer == null)
            {
                _logger.LogError($"UniverseFilter subscribe received a null observer");
                return null;
            }

            if (!_universeObservers.ContainsKey(observer))
            {
                _logger.LogInformation($"UniverseFilter subscribing a new observer");
                _universeObservers.TryAdd(observer, observer);
            }

            return _universeUnsubscriberFactory.Create(_universeObservers, observer);
        }

        public void OnCompleted()
        {
            _logger.LogInformation($"UniverseFilter has received OnCompleted() from the stream. Forwarding to observers.");

            foreach (var obs in _universeObservers)
            {
                obs.Value?.OnCompleted();
            }
        }

        public void OnError(Exception error)
        {
            _logger.LogError($"UniverseFilter OnError() received an exception", error);

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

            _logger.LogInformation($"UniverseFilter is not filtering event at {value.EventTime} with type {value.StateChange}");
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

            if (value.StateChange != UniverseStateEvent.Order
                && value.StateChange != UniverseStateEvent.OrderPlaced)
            {
                return false;
            }

            var frame = (Order)value.UnderlyingEvent;

            if (frame == null)
            {
                return false;
            }

            var filterResult = false;

            switch (_accounts.Type)
            {
                case RuleFilterType.Include:
                    filterResult = !_accounts.Ids.Contains(frame.OrderClientAccountAttributionId, StringComparer.InvariantCultureIgnoreCase);
                    break;
                case RuleFilterType.Exclude:
                    filterResult = _accounts.Ids.Contains(frame.OrderClientAccountAttributionId, StringComparer.InvariantCultureIgnoreCase);
                    break;
            }

            if (filterResult)
            {
                _logger.LogInformation($"UniverseFilter FilterOnAccount filtering out order with id {frame.ReddeerOrderId}");
            }

            return filterResult;
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

            if (value.StateChange != UniverseStateEvent.Order
                && value.StateChange != UniverseStateEvent.OrderPlaced)
            {
                return false;
            }

            var frame = (Order)value.UnderlyingEvent;

            if (frame == null)
            {
                return false;
            }

            var filterResult = false;

            switch (_traders.Type)
            {
                case RuleFilterType.Include:
                    filterResult = !_traders.Ids.Contains(frame.OrderTraderId, StringComparer.InvariantCultureIgnoreCase);
                    break;
                case RuleFilterType.Exclude:
                    filterResult = _traders.Ids.Contains(frame.OrderTraderId, StringComparer.InvariantCultureIgnoreCase);
                    break;
            }

            if (filterResult)
            {
                _logger.LogInformation($"UniverseFilter FilterOnTraders filtering out order with id {frame.ReddeerOrderId}");
            }

            return filterResult;
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

            if (value.StateChange != UniverseStateEvent.Order
                && value.StateChange != UniverseStateEvent.OrderPlaced
                && value.StateChange != UniverseStateEvent.EquityIntradayTick)
            {
                return false;
            }

            if (value.StateChange == UniverseStateEvent.EquityIntradayTick)
            {
                var exchFrame = (MarketTimeBarCollection)value.UnderlyingEvent;
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

                    var filter = !_markets.Ids.Contains(exchFrame.Exchange.MarketIdentifierCode, StringComparer.InvariantCultureIgnoreCase);

                    if (filter)
                    {
                        _logger.LogInformation($"UniverseFilter FilterOnTraders filtering out stock tick with id {exchFrame.Exchange.MarketIdentifierCode} at {exchFrame.Epoch}");
                    }

                    return filter;
                }

                if (_markets.Type == RuleFilterType.Exclude)
                {
                    if (exchFrame?.Exchange?.MarketIdentifierCode == null)
                    {
                        return false;
                    }

                    var filter = _markets.Ids.Contains(exchFrame.Exchange.MarketIdentifierCode, StringComparer.InvariantCultureIgnoreCase);

                    if (filter)
                    {
                        _logger.LogInformation($"UniverseFilter FilterOnTraders filtering out stock tick with id {exchFrame.Exchange.MarketIdentifierCode} at {exchFrame.Epoch}");
                    }

                    return filter;
                }

                return false;
            }

            if (value.StateChange == UniverseStateEvent.Order
                || value.StateChange == UniverseStateEvent.OrderPlaced)
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

                    var filter = !_markets.Ids.Contains(tradeFrame.Market.MarketIdentifierCode, StringComparer.InvariantCultureIgnoreCase);

                    if (filter)
                    {
                        _logger.LogInformation($"UniverseFilter FilterOnMarkets filtering out order with reddeer id of {tradeFrame.ReddeerOrderId}");
                    }

                    return filter;

                }

                if (_markets.Type == RuleFilterType.Exclude)
                {
                    if (tradeFrame?.Market?.MarketIdentifierCode == null)
                    {
                        _logger.LogInformation($"UniverseFilter FilterOnMarkets filtering out order with reddeer id of {tradeFrame.ReddeerOrderId}");

                        return true;
                    }

                    var filter = _markets.Ids.Contains(tradeFrame.Market.MarketIdentifierCode, StringComparer.InvariantCultureIgnoreCase);

                    if (filter)
                    {
                        _logger.LogInformation($"UniverseFilter FilterOnMarkets filtering out order with reddeer id of {tradeFrame.ReddeerOrderId}");
                    }

                    return filter;
                }

                return false;
            }

            return false;
        }

        public DomainV2.Scheduling.Rules Rule { get; } = DomainV2.Scheduling.Rules.UniverseFilter;
        public string Version { get; } = Versioner.Version(0, 0);

        public object Clone()
        {
            _logger.LogInformation($"UniverseFilter Clone called; returning a memberwise clone");
            // we will want to keep the same universe observers here
            return this.MemberwiseClone();
        }
    }
}