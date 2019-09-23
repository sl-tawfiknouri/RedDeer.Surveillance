namespace Domain.Surveillance.Streams
{
    using System;
    using System.Collections.Concurrent;

    using Domain.Core.Markets.Collections;
    using Domain.Surveillance.Streams.Interfaces;

    /// <summary>
    ///     An observable stream of stock exchange time bars
    /// </summary>
    public class ExchangeStream : IStockExchangeStream
    {
        private readonly IUnsubscriberFactory<EquityIntraDayTimeBarCollection> _factory;

        private readonly ConcurrentDictionary<IObserver<EquityIntraDayTimeBarCollection>, IObserver<EquityIntraDayTimeBarCollection>> _observers;

        public ExchangeStream(IUnsubscriberFactory<EquityIntraDayTimeBarCollection> unsubscriberFactory)
        {
            this._observers =
                new ConcurrentDictionary<IObserver<EquityIntraDayTimeBarCollection>,
                    IObserver<EquityIntraDayTimeBarCollection>>();
            this._factory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
        }

        public void Add(EquityIntraDayTimeBarCollection frame)
        {
            if (frame == null) return;

            if (this._observers == null) return;

            foreach (var obs in this._observers)
                obs.Value?.OnNext(frame);
        }

        public IDisposable Subscribe(IObserver<EquityIntraDayTimeBarCollection> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));

            if (!this._observers.ContainsKey(observer)) this._observers.TryAdd(observer, observer);

            return this._factory.Create(this._observers, observer);
        }
    }
}