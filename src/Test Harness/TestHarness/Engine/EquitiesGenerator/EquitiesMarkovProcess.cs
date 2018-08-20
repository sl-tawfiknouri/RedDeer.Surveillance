using NLog;
using System;
using System.Timers;
using System.Linq;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Strategies.Interfaces;
using Domain.Equity.Trading.Streams.Interfaces;
using Domain.Equity.Trading.Frames;

namespace TestHarness.Engine.EquitiesGenerator
{
    /// <summary>
    /// Apply a random walk to securities in your stream
    /// Not multithread safe, only use with transitory life style
    /// </summary>
    public class EquitiesMarkovProcess : IEquityDataGenerator
    {
        private volatile bool _walkInitiated;
        private volatile bool _tickLocked;
        private readonly IExchangeSeriesInitialiser _exchangeTickInitialiser;
        private readonly IEquityDataGeneratorStrategy _dataStrategy;
        private IStockExchangeStream _stream;
        private ExchangeFrame _activeFrame;
        private Timer _activeTimer;

        private readonly ILogger _logger;

        private object _stateTransitionLock = new object();
        private object _walkingLock = new object();

        public EquitiesMarkovProcess(
            IExchangeSeriesInitialiser exchangeTickInitialiser,
            IEquityDataGeneratorStrategy dataStrategy,
            ILogger logger)
        {
            _exchangeTickInitialiser = exchangeTickInitialiser;
            _dataStrategy = dataStrategy;
            _logger = logger;
        }

        public void InitiateWalk(IStockExchangeStream stream, TimeSpan tickFrequency)
        {
            _logger.Log(LogLevel.Info, "Walk initiated in equity generator");

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            lock (_stateTransitionLock)
            {
                ResetTimer();
                _walkInitiated = true;

                _stream = stream;
                _activeFrame = _exchangeTickInitialiser.InitialFrame();
                _stream.Add(_activeFrame);

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
                _logger.Log(LogLevel.Info, "Ticks tocking too fast for equity generator");
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

                var tockedSecurities = _activeFrame
                    .Securities
                    .Select(TickSecurity)
                    .ToArray();

                var tickTock = new ExchangeFrame(_activeFrame.Exchange, tockedSecurities);
                _activeFrame = tickTock;

                _stream.Add(tickTock);

                _tickLocked = false;
            }
        }

        private SecurityFrame TickSecurity(SecurityFrame frame)
        {
            return _dataStrategy.AdvanceFrame(frame);
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
