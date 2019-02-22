﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using Domain.Equity.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Universe
{
    /// <summary>
    /// Play the history of the universe to the observers
    /// </summary>
    public class UniversePlayer : IUniversePlayer
    {
        private readonly IUnsubscriberFactory<IUniverseEvent> _universeUnsubscriberFactory;
        private readonly ILogger<UniversePlayer> _logger;
        private readonly ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>> _universeObservers;

        public UniversePlayer(
            IUnsubscriberFactory<IUniverseEvent> universeUnsubscriberFactory,
            ILogger<UniversePlayer> logger)
        {
            _universeObservers = new ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>>();
            _universeUnsubscriberFactory =
                universeUnsubscriberFactory
                ?? throw new ArgumentNullException(nameof(universeUnsubscriberFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Play(IUniverse universe)
        {
            if (universe == null)
            {
                _logger.LogError($"UniversePlayer added to play null universe. Returning.");
                return;
            }

            PlayUniverse(universe);
        }

        private void PlayUniverse(IUniverse universe)
        {
            if (_universeObservers == null
                || !_universeObservers.Any())
            {
                _logger.LogError($"UniversePlayer play universe to null or empty observers. Returning");

                // does a tree fall in an empty forest? no
                return;
            }

            _logger.LogInformation($"UniversePlayer play universe about to distribute all universe events to all observers");
            foreach (var item in universe.UniverseEvents)
            {
                foreach (var obs in _universeObservers)
                {
                    obs.Value?.OnNext(item);
                }
            }
        }

        /// <summary>
        /// Subscribe to the universe
        /// </summary>
        public IDisposable Subscribe(IObserver<IUniverseEvent> observer)
        {
            if (observer == null)
            {
                _logger.LogError($"UniversePlayer asked to subscribe a null observer");
                return null;
            }

            if (!_universeObservers.ContainsKey(observer))
            {
                _logger.LogInformation($"UniversePlayer subscribing a new observer");
                _universeObservers.TryAdd(observer, observer);
            }

            return _universeUnsubscriberFactory.Create(_universeObservers, observer);
        }
    }
}