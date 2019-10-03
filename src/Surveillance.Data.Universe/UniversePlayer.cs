namespace Surveillance.Data.Universe
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;

    using Domain.Surveillance.Streams.Interfaces;

    using Microsoft.Extensions.Logging;

    using Surveillance.Data.Universe.Interfaces;

    /// <summary>
    ///     Play the history of the universe to the observers
    /// </summary>
    public class UniversePlayer : IUniversePlayer
    {
        /// <summary>
        /// The cancellation token.
        /// </summary>
        private readonly CancellationToken cancellationToken;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<UniversePlayer> logger;

        /// <summary>
        /// The universe observers.
        /// </summary>
        private readonly ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>> universeObservers;

        /// <summary>
        /// The universe unsubscriber factory.
        /// </summary>
        private readonly IUnsubscriberFactory<IUniverseEvent> universeUnsubscriberFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="UniversePlayer"/> class.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <param name="universeUnsubscriberFactory">
        /// The universe unsubscriber factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public UniversePlayer(
            CancellationToken cancellationToken,
            IUnsubscriberFactory<IUniverseEvent> universeUnsubscriberFactory,
            ILogger<UniversePlayer> logger)
        {
            this.cancellationToken = cancellationToken;
            this.universeObservers = new ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>>();
            this.universeUnsubscriberFactory = 
                universeUnsubscriberFactory ?? throw new ArgumentNullException(nameof(universeUnsubscriberFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The play.
        /// </summary>
        /// <param name="universe">
        /// The universe.
        /// </param>
        public void Play(IUniverse universe)
        {
            if (universe == null)
            {
                this.logger.LogError("added to play null universe. Returning.");
                return;
            }

            this.PlayUniverse(universe);
        }

        /// <summary>
        /// Subscribe to the universe
        /// </summary>
        /// <param name="observer">
        /// The observer.
        /// </param>
        /// <returns>
        /// The <see cref="IDisposable"/>.
        /// </returns>
        public IDisposable Subscribe(IObserver<IUniverseEvent> observer)
        {
            if (!this.universeObservers.ContainsKey(observer))
            {
                this.logger.LogInformation("subscribing a new observer");
                this.universeObservers.TryAdd(observer, observer);
            }

            return this.universeUnsubscriberFactory.Create(this.universeObservers, observer);
        }

        /// <summary>
        /// The play universe.
        /// </summary>
        /// <param name="universe">
        /// The universe.
        /// </param>
        private void PlayUniverse(IUniverse universe)
        {
            if (this.universeObservers == null || !this.universeObservers.Any())
            {
                this.logger.LogError("play universe to null or empty observers. Returning");
                return;
            }

            this.logger.LogInformation("play universe about to distribute all universe events to all observers");
            foreach (var item in universe.UniverseEvents)
            {
                foreach (var obs in this.universeObservers)
                {
                    obs.Value?.OnNext(item);
                }

                if (this.cancellationToken.IsCancellationRequested)
                {
                    this.logger.LogInformation(
                        $"play universe cancelled - breaking at event {item.EventTime} {item.StateChange}");
                    break;
                }
            }
        }
    }
}