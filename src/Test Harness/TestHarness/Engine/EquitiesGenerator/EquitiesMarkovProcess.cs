using NLog;
using System;
using System.Linq;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Strategies.Interfaces;
using Domain.Equity.Trading.Streams.Interfaces;
using Domain.Equity.Trading.Frames;
using TestHarness.Engine.Heartbeat.Interfaces;

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

        private IHeartbeat _heartBeat;

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

        public void InitiateWalk(IStockExchangeStream stream, IHeartbeat heartBeat)
        {
            _logger.Log(LogLevel.Info, "Walk initiated in equity generator");

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (heartBeat == null)
            {
                throw new ArgumentNullException(nameof(heartBeat));
            }

            lock (_stateTransitionLock)
            {
                _walkInitiated = true;

                _stream = stream;
                _activeFrame = _exchangeTickInitialiser.InitialFrame();
                _stream.Add(_activeFrame);

                _heartBeat = heartBeat;
                _heartBeat.OnBeat(Tick);
                _heartBeat.Start();
            }
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
                    if (_heartBeat != null)
                    {
                        _heartBeat.Stop();
                    }
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

                _heartBeat.Stop();
                _heartBeat.Dispose();

                _logger.Log(LogLevel.Info, "Random walk generator terminating walk");
            }
        }
    }
}
