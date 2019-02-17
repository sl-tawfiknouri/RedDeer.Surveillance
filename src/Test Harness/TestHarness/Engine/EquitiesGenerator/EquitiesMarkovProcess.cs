using System;
using System.Linq;
using Domain.Equity.Streams.Interfaces;
using Domain.Equity.TimeBars;
using Microsoft.Extensions.Logging;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Strategies.Interfaces;
using TestHarness.Engine.Heartbeat.Interfaces;

namespace TestHarness.Engine.EquitiesGenerator
{
    /// <summary>
    /// Apply a random walk to securities in your stream
    /// Not multi thread safe, only use with transitory life style
    /// </summary>
    public class EquitiesMarkovProcess : IEquityDataGenerator
    {
        private volatile bool _walkInitiated;
        private volatile bool _tickLocked;

        private readonly IExchangeSeriesInitialiser _exchangeTickInitialiser;
        private readonly IEquityDataGeneratorStrategy _dataStrategy;
        private IStockExchangeStream _stream;
        private EquityIntraDayTimeBarCollection _activeFrame;
        private readonly IHeartbeat _heartBeat;

        private readonly ILogger _logger;

        private readonly object _stateTransitionLock = new object();
        private readonly object _walkingLock = new object();

        public EquitiesMarkovProcess(
            IExchangeSeriesInitialiser exchangeTickInitialiser,
            IEquityDataGeneratorStrategy dataStrategy,
            IHeartbeat heartbeat,
            ILogger logger)
        {
            _exchangeTickInitialiser =
                exchangeTickInitialiser 
                ?? throw new ArgumentNullException(nameof(exchangeTickInitialiser));

            _dataStrategy =
                dataStrategy
                ?? throw new ArgumentNullException(nameof(dataStrategy));

            _heartBeat =
                heartbeat
                ?? throw new ArgumentNullException(nameof(heartbeat));

            _logger =
                logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public void InitiateWalk(IStockExchangeStream stream)
        {
            _logger.LogInformation("Walk initiated in equity generator");

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            lock (_stateTransitionLock)
            {
                _walkInitiated = true;

                _stream = stream;
                _activeFrame = _exchangeTickInitialiser.InitialFrame();
                _stream.Add(_activeFrame);

                _heartBeat.OnBeat(Tick);
                _heartBeat.Start();
            }
        }

        private void Tick(object sender, EventArgs e)
        {
            if (_tickLocked)
            {
                // don't tick if we're getting overwhelmed
                _logger.LogInformation("Ticks tocking too fast for equity generator");
                return;
            }

            lock (_walkingLock)
            {
                _tickLocked = true;

                if (!_walkInitiated)
                {
                    _heartBeat?.Stop();
                    _tickLocked = false;
                    return;
                }

                var tockedSecurities = _activeFrame
                    .Securities
                    .Select(TickSecurity)
                    .ToArray();

                var tickTock = new EquityIntraDayTimeBarCollection(_activeFrame.Exchange, DateTime.UtcNow, tockedSecurities);
                _activeFrame = tickTock;

                _stream.Add(tickTock);

                _tickLocked = false;
            }
        }

        private EquityInstrumentIntraDayTimeBar TickSecurity(EquityInstrumentIntraDayTimeBar tick)
        {
            return _dataStrategy.AdvanceFrame(tick, DateTime.UtcNow, true);
        }

        public void TerminateWalk()
        {
            lock (_stateTransitionLock)
            {
                _walkInitiated = false;

                _heartBeat.Stop();
                _heartBeat.Dispose();

                _logger.LogInformation("Random walk generator terminating walk");
            }
        }
    }
}
