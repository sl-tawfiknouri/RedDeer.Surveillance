using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Equity.Frames;
using DomainV2.Equity.Streams.Interfaces;
using DomainV2.Financial;
using Surveillance.Rules.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.MarketEvents;
using Surveillance.Universe.Multiverse.Interfaces;

namespace Surveillance.Universe.Multiverse
{
    /// <summary>
    /// Substitutes intraday exchange frame updates with the ultimate update
    /// We allow updates until 4 hours before the next market open
    /// </summary>
    public class MarketCloseMultiverseTransformer : IMarketCloseMultiverseTransformer
    {
        private readonly IUnsubscriberFactory<IUniverseEvent> _universeUnsubscriberFactory;
        private readonly ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>> _universeObservers;
        private readonly IDictionary<string, List<FrameToDate>> _exchangeFrame;
        private readonly Queue<IUniverseEvent> _universeEvents;
        private readonly object _lock = new object();

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private IList<IUniverseCloneableRule> _subscribedRules;

        public MarketCloseMultiverseTransformer(IUnsubscriberFactory<IUniverseEvent> unsubscriberFactory)
        {
            _universeObservers = new ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>>();
            _universeUnsubscriberFactory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
            _exchangeFrame = new Dictionary<string, List<FrameToDate>>();
            _universeEvents = new Queue<IUniverseEvent>();
            _subscribedRules = new List<IUniverseCloneableRule>();
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
            lock (_lock)
            {
                switch (value.StateChange)
                {
                    case UniverseStateEvent.StockTickReddeer:
                        StockTick(value);
                        break;
                    case UniverseStateEvent.StockMarketOpen:
                        StockMarketOpen(value);
                        break;
                }

                _universeEvents.Enqueue(value);
                UnloadQueue(value.EventTime, value.StateChange == UniverseStateEvent.Eschaton);
            }
        }

        private void StockTick(IUniverseEvent universeEvent)
        {
            if (universeEvent == null
                || universeEvent.StateChange != UniverseStateEvent.StockTickReddeer)
            {
                return;
            }

             var frame = (ExchangeFrame)universeEvent.UnderlyingEvent;

            if (_exchangeFrame.ContainsKey(frame.Exchange.MarketIdentifierCode))
            {
                _exchangeFrame.TryGetValue(frame.Exchange.MarketIdentifierCode, out var timeFrame);

                var tfEntry =
                    timeFrame
                        ?.Where(tf => tf.OpenDate > frame.TimeStamp)
                        .OrderBy(tf => tf.OpenDate)
                        .FirstOrDefault();

                if (tfEntry == null)
                {
                    return;
                }

                tfEntry.Frame = frame;
            }
        }

        private void StockMarketOpen(IUniverseEvent universeEvent)
        {
            var marketOpenClose = (MarketOpenClose) universeEvent?.UnderlyingEvent;

            if (marketOpenClose == null)
            {
                return;
            }

            if (!_exchangeFrame.ContainsKey(marketOpenClose.MarketId))
            {
                _exchangeFrame.Add(
                    marketOpenClose.MarketId,
                    new List<FrameToDate>
                    {
                        new FrameToDate { OpenDate = marketOpenClose.MarketOpen.AddHours(-4), Frame =  null},
                        new FrameToDate { OpenDate = marketOpenClose.MarketOpen.AddDays(1).AddHours(-4), Frame =  null}
                    });

                return;
            }

            _exchangeFrame.TryGetValue(marketOpenClose.MarketId, out var frames);
            frames?.Add(new FrameToDate { OpenDate = marketOpenClose.MarketOpen.AddDays(1).AddHours(-4), Frame = null});           
        }

        private void UnloadQueue(DateTime tailOfQueue, bool flushQueue)
        {
            if (flushQueue)
            {
                while (_universeEvents.Count > 0)
                {
                    var @event = _universeEvents.Dequeue();
                    @event = SwapIfFrame(@event);

                    foreach (var obs in _universeObservers)
                    {
                        obs.Value?.OnNext(@event);
                    }
                }

                return;
            }

            while (_universeEvents.Peek().EventTime < tailOfQueue.Subtract(TimeSpan.FromHours(20)))
            {
                var @event = _universeEvents.Dequeue();
                @event = SwapIfFrame(@event);

                foreach (var obs in _universeObservers)
                {
                    obs.Value?.OnNext(@event);
                }
            }
        }

        private IUniverseEvent SwapIfFrame(IUniverseEvent universeEvent)
        {
            if (universeEvent.StateChange != UniverseStateEvent.StockTickReddeer)
            {
                return universeEvent;
            }

            var frame = (ExchangeFrame) universeEvent.UnderlyingEvent;
            _exchangeFrame.TryGetValue(frame.Exchange.MarketIdentifierCode, out var frames);

            if (frames == null)
            {
                return universeEvent;
            }

            var updateFrame =
                frames
                    .Where(fra => fra.OpenDate > frame.TimeStamp)
                    .OrderBy(fra => fra.OpenDate)
                    .FirstOrDefault(fra => fra.Frame != null) 
                    ?.Frame;

            return
                updateFrame == null
                ? universeEvent
                : new UniverseEvent(universeEvent.StateChange, universeEvent.EventTime, updateFrame);
        }

        public IDisposable Subscribe(IUniverseCloneableRule rule)
        {
            _subscribedRules?.Add(rule);

            return Subscribe(rule as IObserver<IUniverseEvent>);
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

        public void RemoveSubscribers(IList<IUniverseCloneableRule> rules)
        {
            if (rules == null
                || !rules.Any())
            {
                return;
            }

            foreach (var rule in rules)
            {
                _universeObservers.TryRemove(rule, out var obs);
            }
        }

        public void ResetRuleSubscribers()
        {
            if (_subscribedRules == null
                || !_subscribedRules.Any())
            {
                return;
            }

            _subscribedRules.Clear();
        }

        private class FrameToDate
        {
            /// <summary>
            /// Last call for updates relating to the prior period i.e. a date book end
            /// </summary>
            public DateTime OpenDate { get; set; }
            
            public ExchangeFrame Frame { get; set; }
        }
        
        public object Clone()
        {
            var initialClone = (MarketCloseMultiverseTransformer)this.MemberwiseClone();

            _subscribedRules = new List<IUniverseCloneableRule>(_subscribedRules); // change the list by reference
            initialClone.RemoveSubscribers(_subscribedRules);
            initialClone.ResetRuleSubscribers();

            var subscriberClones =
                _subscribedRules
                    .Select(subs => (IUniverseCloneableRule)subs.Clone())
                    .ToList();

            foreach (var subClone in subscriberClones)
                initialClone.Subscribe(subClone);

            return initialClone;
        }
    }
}