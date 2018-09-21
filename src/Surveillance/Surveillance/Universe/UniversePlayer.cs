using System;
using System.Collections.Concurrent;
using System.Linq;
using Domain.Equity.Frames;
using Domain.Equity.Streams.Interfaces;
using Domain.Trades.Orders;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe
{
    /// <summary>
    /// Play the history of the universe to the observers
    /// </summary>
    public class UniversePlayer : IUniversePlayer
    {
        private readonly IUnsubscriberFactory<TradeOrderFrame> _tradeUnsubscriberFactory;
        private readonly ConcurrentDictionary<IObserver<TradeOrderFrame>, IObserver<TradeOrderFrame>> _tradeObservers;

        private readonly IUnsubscriberFactory<ExchangeFrame> _exchangeUnsubscriberFactory;
        private readonly ConcurrentDictionary<IObserver<ExchangeFrame>, IObserver<ExchangeFrame>> _exchangeObservers;

        public UniversePlayer(
            IUnsubscriberFactory<TradeOrderFrame> tradeUnsubscriberFactory,
            IUnsubscriberFactory<ExchangeFrame> exchangeUnsubscriberFactory)
        {
            _tradeObservers = new ConcurrentDictionary<IObserver<TradeOrderFrame>, IObserver<TradeOrderFrame>>();
            _tradeUnsubscriberFactory =
                tradeUnsubscriberFactory
                ?? throw new ArgumentNullException(nameof(tradeUnsubscriberFactory));

            _exchangeObservers = new ConcurrentDictionary<IObserver<ExchangeFrame>, IObserver<ExchangeFrame>>();
            _exchangeUnsubscriberFactory =
                exchangeUnsubscriberFactory
                ?? throw new ArgumentNullException(nameof(exchangeUnsubscriberFactory));
        }

        public void Play(IUniverse universe)
        {
            if (universe == null
                || !universe.Trades.Any())
            {
                return;
            }

            PlayTradesOnly(universe);
            PlayExchangeOnly(universe);
        }

        private void PlayTradesOnly(IUniverse universe)
        {
            if (_tradeObservers == null
                || !_tradeObservers.Any())
            {
                return;
            }

            foreach (var item in universe.Trades)
            {
                foreach (var obs in _tradeObservers)
                {
                    obs.Value?.OnNext(item);
                }
            }
        }

        private void PlayExchangeOnly(IUniverse universe)
        {
            if (_exchangeObservers == null
                || !_exchangeObservers.Any())
            {
                return;
            }

            foreach (var item in universe.MarketEquityData)
            {
                foreach (var obs in _exchangeObservers)
                {
                    obs.Value?.OnNext(item);
                }
            }
        }

        /// <summary>
        /// Subscribe to the trades only
        /// </summary>
        public IDisposable Subscribe(IObserver<TradeOrderFrame> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            if (!_tradeObservers.ContainsKey(observer))
            {
                _tradeObservers.TryAdd(observer, observer);
            }

            return _tradeUnsubscriberFactory.Create(_tradeObservers, observer);
        }

        /// <summary>
        /// Subscribe to exchange data only
        /// </summary>
        public IDisposable Subscribe(IObserver<ExchangeFrame> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            if (_exchangeObservers.ContainsKey(observer))
            {
                _exchangeObservers.TryAdd(observer, observer);
            }

            return _exchangeUnsubscriberFactory.Create(_exchangeObservers, observer);
        }
    }
}
