using System;
using System.Collections.Concurrent;
using System.Linq;
using Domain.Core.Markets.Collections;
using Domain.Core.Trading.Orders;
using Domain.Surveillance.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Filter
{
    public class UniverseFilterService : IUniverseFilterService
    {
        private readonly IUnsubscriberFactory<IUniverseEvent> _universeUnsubscriberFactory;
        private readonly ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>> _universeObservers;

        private readonly RuleFilter _accounts;
        private readonly RuleFilter _traders;
        private readonly RuleFilter _markets;
        private readonly RuleFilter _funds;
        private readonly RuleFilter _strategies;
        private readonly ILogger<UniverseFilterService> _logger;

        public UniverseFilterService(
            IUnsubscriberFactory<IUniverseEvent> universeUnsubscriberFactory,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets,
            RuleFilter funds,
            RuleFilter strategies,
            ILogger<UniverseFilterService> logger)
        {
            _universeUnsubscriberFactory =
                universeUnsubscriberFactory
                ?? throw new ArgumentNullException(nameof(universeUnsubscriberFactory));

            _accounts = accounts;
            _traders = traders;
            _markets = markets;
            _funds = funds;
            _strategies = strategies;

            _universeObservers = new ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>>();

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Domain.Surveillance.Scheduling.Rules Rule { get; } = Domain.Surveillance.Scheduling.Rules.UniverseFilter;
        public string Version { get; } = Versioner.Version(0, 0);

        public IDisposable Subscribe(IObserver<IUniverseEvent> observer)
        {
            if (!_universeObservers.ContainsKey(observer))
            {
                _logger.LogInformation($"subscribing a new observer");
                _universeObservers.TryAdd(observer, observer);
            }

            return _universeUnsubscriberFactory.Create(_universeObservers, observer);
        }
        
        public void OnCompleted()
        {
            _logger.LogInformation($"has received OnCompleted() from the stream. Forwarding to observers.");

            foreach (var obs in _universeObservers)
            {
                obs.Value?.OnCompleted();
            }
        }

        public void OnError(Exception error)
        {
            _logger.LogError($"OnError() received an exception", error);

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

            if (FilterOnFund(value))
            {
                return;
            }

            if (FilterOnStrategy(value))
            {
                return;
            }

            _logger.LogInformation($"is not filtering event at {value.EventTime} with type {value.StateChange}");
            foreach (var obs in _universeObservers)
            {
                obs.Value?.OnNext(value);
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

            if (value.StateChange != UniverseStateEvent.Order
                && value.StateChange != UniverseStateEvent.OrderPlaced
                && value.StateChange != UniverseStateEvent.EquityIntradayTick)
            {
                return false;
            }

            if (value.StateChange == UniverseStateEvent.EquityIntradayTick)
            {
                return FilterOnMarketIntradayTick(value);
            }

            if (value.StateChange == UniverseStateEvent.Order
                || value.StateChange == UniverseStateEvent.OrderPlaced)
            {
                return FilterOnOrderAttribute(value, _markets, i => i?.Market?.MarketIdentifierCode, "market");
            }

            return false;
        }

        private bool FilterOnMarketIntradayTick(IUniverseEvent value)
        {
            var exchFrame = (EquityIntraDayTimeBarCollection)value.UnderlyingEvent;
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
                    _logger.LogInformation($"filtering out stock tick with id {exchFrame.Exchange.MarketIdentifierCode} at {exchFrame.Epoch}");
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
                    _logger.LogInformation($"filtering out stock tick with id {exchFrame.Exchange.MarketIdentifierCode} at {exchFrame.Epoch}");
                }

                return filter;
            }

            return false;
        }

        private bool FilterOnAccount(IUniverseEvent value)
        {
            return FilterOnOrderAttribute(value, _accounts, i => i.OrderClientAccountAttributionId, "Accounts");
        }

        private bool FilterOnTraders(IUniverseEvent value)
        {
            return FilterOnOrderAttribute(value, _traders, i => i.OrderTraderId, "Trader");
        }
        private bool FilterOnFund(IUniverseEvent value)
        {
            return FilterOnOrderAttribute(value, _funds, i => i.OrderFund, "fund");
        }

        private bool FilterOnStrategy(IUniverseEvent value)
        {
            return FilterOnOrderAttribute(value, _strategies, i => i.OrderStrategy, "strategy");
        }

        private bool FilterOnOrderAttribute(IUniverseEvent value, RuleFilter filter, Func<Order, string> propertyLens, string filterName)
        {
            if (filter == null)
            {
                return false;
            }

            if (filter.Type == RuleFilterType.None)
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

            switch (filter.Type)
            {
                case RuleFilterType.Include:
                    filterResult = !filter.Ids.Contains(propertyLens(frame), StringComparer.InvariantCultureIgnoreCase);
                    break;
                case RuleFilterType.Exclude:
                    filterResult = filter.Ids.Contains(propertyLens(frame), StringComparer.InvariantCultureIgnoreCase);
                    break;
            }

            if (filterResult)
            {
                _logger.LogInformation($"{filterName} filtering out order with id {frame.ReddeerOrderId}");
            }

            return filterResult;
        }
    }
}