namespace Surveillance.Engine.Rules.Universe
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;

    using Domain.Surveillance.Streams.Interfaces;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Universe.Interfaces;

    /// <summary>
    ///     Play the history of the universe to the observers
    /// </summary>
    public class UniversePlayer : IUniversePlayer
    {
        private readonly CancellationToken _ct;

        private readonly ILogger<UniversePlayer> _logger;

        private readonly ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>> _universeObservers;

        private readonly IUnsubscriberFactory<IUniverseEvent> _universeUnsubscriberFactory;

        public UniversePlayer(
            CancellationToken ct,
            IUnsubscriberFactory<IUniverseEvent> universeUnsubscriberFactory,
            ILogger<UniversePlayer> logger)
        {
            this._ct = ct;
            this._universeObservers = new ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>>();
            this._universeUnsubscriberFactory = universeUnsubscriberFactory
                                                ?? throw new ArgumentNullException(nameof(universeUnsubscriberFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Play(IUniverse universe)
        {
            if (universe == null)
            {
                this._logger.LogError("added to play null universe. Returning.");
                return;
            }

            this.PlayUniverse(universe);
        }

        /// <summary>
        ///     Subscribe to the universe
        /// </summary>
        public IDisposable Subscribe(IObserver<IUniverseEvent> observer)
        {
            if (!this._universeObservers.ContainsKey(observer))
            {
                this._logger.LogInformation("subscribing a new observer");
                this._universeObservers.TryAdd(observer, observer);
            }

            return this._universeUnsubscriberFactory.Create(this._universeObservers, observer);
        }

        private void PlayUniverse(IUniverse universe)
        {
            if (this._universeObservers == null || !this._universeObservers.Any())
            {
                this._logger.LogError("play universe to null or empty observers. Returning");
                return;
            }

            this._logger.LogInformation("play universe about to distribute all universe events to all observers");
            foreach (var item in universe.UniverseEvents)
            {
                foreach (var obs in this._universeObservers) obs.Value?.OnNext(item);

                if (this._ct.IsCancellationRequested)
                {
                    this._logger.LogInformation(
                        $"play universe cancelled - breaking at event {item.EventTime} {item.StateChange}");
                    break;
                }
            }
        }
    }
}