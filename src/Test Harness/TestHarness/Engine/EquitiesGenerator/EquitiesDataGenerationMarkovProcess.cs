using System;
using System.Linq;
using Domain.Equity.Frames;
using Domain.Equity.Streams.Interfaces;
using NLog;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using RedDeer.Contracts.SurveillanceService.Api.SecurityPrices;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Strategies.Interfaces;

namespace TestHarness.Engine.EquitiesGenerator
{
    /// <summary>
    /// Apply a random walk to securities in your stream
    /// Not multi thread safe, only use with transitory life style
    /// </summary>
    public class EquitiesDataGenerationMarkovProcess : IEquitiesDataGenerationMarkovProcess
    {
        private volatile bool _walkInitiated;

        private readonly IEquityDataGeneratorStrategy _dataStrategy;
        private IStockExchangeStream _stream;
        private ExchangeFrame _activeFrame;
        private readonly ILogger _logger;

        private readonly object _stateTransitionLock = new object();
        private readonly object _walkingLock = new object();

        private readonly TimeSpan _tickSeparation;

        public EquitiesDataGenerationMarkovProcess(
            IEquityDataGeneratorStrategy dataStrategy,
            TimeSpan tickSeparation,
            ILogger logger)
        {
            _dataStrategy =
                dataStrategy
                ?? throw new ArgumentNullException(nameof(dataStrategy));

            _tickSeparation = tickSeparation;

            _logger =
                logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public void InitiateWalk(IStockExchangeStream stream, ExchangeDto market, SecurityPriceResponseDto prices)
        {
            _logger.Log(LogLevel.Info, "Walk initiated in equity generator");

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (market == null)
            {
                throw new ArgumentNullException(nameof(market));
            }

            if (prices == null)
            {
                throw new ArgumentNullException(nameof(prices));
            }

            lock (_stateTransitionLock)
            {
                _walkInitiated = true;
                _stream = stream;

                var apiGeneration = new ApiDataGenerationInitialiser(market, prices.SecurityPrices);
                var framesToGenerateFor = apiGeneration.OrderedDailyFrames();

                foreach (var frame in framesToGenerateFor)
                {
                    _activeFrame = frame;
                    _stream.Add(_activeFrame);
                    AdvanceFrames(market, frame);
                }
            }
        }

        private void AdvanceFrames(ExchangeDto market, ExchangeFrame frame)
        {
            if (frame.TimeStamp.TimeOfDay > market.MarketCloseTime)
            {
                _logger.Log(LogLevel.Error, "ended up advancing a frame whose start exceeded market close time");
                return;
            }

            var advanceTick = frame.TimeStamp.TimeOfDay.Add(_tickSeparation);

            while (advanceTick <= market.MarketCloseTime)
            {
                Tick(frame.TimeStamp.Date.Add(advanceTick));
                advanceTick = advanceTick.Add(_tickSeparation);
            }
        }

        private void Tick(DateTime advanceTick)
        {
            lock (_walkingLock)
            {
                var tockedSecurities = _activeFrame
                    .Securities
                    .Select(sec => TickSecurity(sec, advanceTick))
                    .ToArray();

                var tickTock = new ExchangeFrame(_activeFrame.Exchange, advanceTick, tockedSecurities);
                _activeFrame = tickTock;

                _stream.Add(tickTock);
            }
        }

        private SecurityTick TickSecurity(SecurityTick tick, DateTime advanceTick)
        {
            return _dataStrategy.AdvanceFrame(tick, advanceTick, false);
        }

        public void TerminateWalk()
        {
            lock (_stateTransitionLock)
            {
                _walkInitiated = false;
                _logger.Log(LogLevel.Info, "Random walk generator terminating walk");
            }
        }
    }
}
