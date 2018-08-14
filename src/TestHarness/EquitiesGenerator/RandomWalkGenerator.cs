using Domain.Equity.Trading;
using NLog;
using System;
using System.Timers;
using System.Linq;

namespace TestHarness.EquitiesGenerator
{
    /// <summary>
    /// Apply a random walk to securities in your stream
    /// Not multithread safe, only use with transitory life style
    /// </summary>
    public class RandomWalkGenerator
    {
        private volatile bool _walkInitiated;
        private volatile bool _tickLocked;
        private readonly IExchangeTickInitialiser _exchangeTickInitialiser;
        private IStockExchangeStream _stream;
        private ExchangeTick _activeTick;
        private Timer _activeTimer;
        private Random _random;

        private readonly ILogger _logger;

        private object _stateTransitionLock = new object();
        private object _walkingLock = new object();

        public RandomWalkGenerator(IExchangeTickInitialiser exchangeTickInitialiser, ILogger logger)
        {
            _exchangeTickInitialiser = exchangeTickInitialiser;
            _random = new Random();
            _logger = logger;
        }

        public void InitiateWalk(IStockExchangeStream stream, TimeSpan tickFrequency)
        {
            _logger.Log(LogLevel.Info, "Walk initiated in random walk generator");

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            lock (_stateTransitionLock)
            {
                ResetTimer();
                _walkInitiated = true;

                _stream = stream;
                _activeTick = _exchangeTickInitialiser.InitialTick();
                _stream.Add(_activeTick);

                SetNewTimer(tickFrequency);
            }
        }

        private void ResetTimer()
        {
            if (_activeTimer != null)
            {
                _activeTimer.Stop();
                _activeTimer.Dispose();
                _activeTimer = null;
            }
        }

        private void SetNewTimer(TimeSpan tickFrequency)
        {
            _activeTimer = new Timer();
            _activeTimer.Interval = tickFrequency.TotalMilliseconds;
            _activeTimer.Elapsed += Tick;
            _activeTimer.AutoReset = true;

            _activeTimer.Enabled = true;
        }

        private void Tick(object sender, EventArgs e)
        {
            if (_tickLocked)
            {
                // don't tick if we're getting overwhelmed
                _logger.Log(LogLevel.Info, "Ticks tocking too fast for random walk generator");
                return;
            }

            lock (_walkingLock)
            {
                _tickLocked = true;

                if (!_walkInitiated)
                {
                    _activeTimer.Stop();
                    _tickLocked = false;
                    return;
                }

                var tockedSecurities = _activeTick
                    .Securities
                    .Select(TickSecurity)
                    .ToArray();

                var tickTock = new ExchangeTick(_activeTick.Exchange, tockedSecurities);
                _activeTick = tickTock;

                _stream.Add(tickTock);

                _tickLocked = false;
            }
        }

        private SecurityTick TickSecurity(SecurityTick tick)
        {
            var newPriceRange = (int)((tick.Spread.Buy.Value) * 0.01m);
            var newPriceOffset = _random.Next(-newPriceRange, newPriceRange);
            var newBuy = tick.Spread.Buy.Value + newPriceOffset;
            var newSell = tick.Spread.Sell.Value + newPriceOffset;
            var newSpread = new Spread(new Price(newBuy), new Price(newSell));

            var newVolumeRange = (int)((tick.Volume.Traded) * 0.05m);
            var newVolumeOffset = _random.Next(-newVolumeRange, newVolumeRange);
            var newVolume = new Volume(tick.Volume.Traded + newVolumeOffset);

            return new SecurityTick(tick.Security, newSpread, newVolume);
        }

        public void TerminateWalk()
        {
            lock (_stateTransitionLock)
            {
                _walkInitiated = false;

                _activeTimer.Stop();
                _activeTimer.Dispose();

                _logger.Log(LogLevel.Info, "Random walk generator terminating walk");
            }
        }
    }
}
