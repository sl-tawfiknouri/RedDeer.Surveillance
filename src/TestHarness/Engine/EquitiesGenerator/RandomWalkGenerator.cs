using Domain.Equity.Trading;
using NLog;
using System;
using System.Timers;
using System.Linq;
using MathNet.Numerics.Distributions;

namespace TestHarness.Engine.EquitiesGenerator
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
        private double _standardDeviation = 0.6; // good value for 15 minute tick updates

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

        /// <summary>
        /// TODO refactor to geometric brownian motion generator
        /// </summary>
        /// <param name="tick"></param>
        /// <returns></returns>
        private SecurityTick TickSecurity(SecurityTick tick)
        {
            var newBuy = (decimal)Normal.Sample((double)tick.Spread.Buy.Value, _standardDeviation);

            if (newBuy < 0.001m)
            {
                newBuy = 0.001m;
            }

            var newSellSample = (decimal)Normal.Sample((double)tick.Spread.Sell.Value, _standardDeviation);

            var newSellLimit = Math.Min(newBuy, newSellSample);
            var newSellFloor = (newBuy * 0.95m); // allow for a max of 5% spread
            var newSell = newSellFloor > newSellLimit ? newSellFloor : newSellLimit;

            var newSpread = new Spread(new Price(newBuy), new Price(newSell));

            var newVolumeSample = (int)Normal.Sample((double)tick.Volume.Traded, _standardDeviation);
            var newVolumeSampleFloor = Math.Max(0, newVolumeSample);
            var newVolume = new Volume(newVolumeSampleFloor);

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
