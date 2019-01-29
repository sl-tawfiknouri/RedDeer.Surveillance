using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Equity.Streams.Interfaces;
using DomainV2.Equity.TimeBars;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using RedDeer.Contracts.SurveillanceService.Api.SecurityPrices;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Strategies;
using TestHarness.Engine.EquitiesGenerator.Strategies.Interfaces;
using TestHarness.Engine.Plans;

namespace TestHarness.Engine.EquitiesGenerator
{
    /// <summary>
    /// Apply a random walk to securities in your stream
    /// Not multi thread safe, only use with transitory life style
    /// </summary>
    public class EquitiesDataGenerationMarkovProcess : IEquitiesDataGenerationMarkovProcess
    {
        private readonly IReadOnlyCollection<DataGenerationPlan> _plan;
        private readonly IEquityDataGeneratorStrategy _dataStrategy;
        private IStockExchangeStream _stream;
        private EquityIntraDayTimeBarCollection _activeFrame;
        private readonly ILogger _logger;

        private readonly object _stateTransitionLock = new object();
        private readonly object _walkingLock = new object();

        private readonly TimeSpan _tickSeparation;

        public EquitiesDataGenerationMarkovProcess(
            IEquityDataGeneratorStrategy dataStrategy,
            IReadOnlyCollection<DataGenerationPlan> plan,
            TimeSpan tickSeparation,
            ILogger logger)
        {
            _dataStrategy =
                dataStrategy
                ?? throw new ArgumentNullException(nameof(dataStrategy));

            _tickSeparation = tickSeparation;
            _plan = plan ?? new List<DataGenerationPlan>();

            _logger =
                logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public void InitiateWalk(IStockExchangeStream stream, ExchangeDto market, SecurityPriceResponseDto prices)
        {
            _logger.LogInformation("Walk initiated in equity generator");

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
                _stream = stream;

                var apiGeneration = new ApiDataGenerationInitialiser(market, _tickSeparation, prices.SecurityPrices);
                var framesToGenerateFor = apiGeneration.OrderedDailyFrames();

                foreach (var frame in framesToGenerateFor)
                {
                    _activeFrame = frame;
                    _stream.Add(_activeFrame);
                    AdvanceFrames(market, frame);
                }
            }
        }

        private void AdvanceFrames(ExchangeDto market, EquityIntraDayTimeBarCollection frame)
        {
            if (frame.Epoch.TimeOfDay > market.MarketCloseTime)
            {
                _logger.Log(LogLevel.Error, "ended up advancing a frame whose start exceeded market close time");
                return;
            }

            var timeSpanList = new List<TickStrategy>();
            var advanceTick = frame.Epoch.TimeOfDay;

            while (advanceTick <= market.MarketCloseTime)
            {
                timeSpanList.Add(
                    new TickStrategy
                    {
                        TickOffset = advanceTick,
                        Strategy = _dataStrategy
                    });

                advanceTick = advanceTick.Add(_tickSeparation);
            }

            timeSpanList = RemoveConflictingTicks(timeSpanList);

            foreach (var subPlan in _plan)
            {
                var strategy = new PlanEquityStrategy(subPlan, _dataStrategy);
                var initialTick = subPlan.EquityInstructions.IntervalCommencement;
                while (initialTick <= subPlan.EquityInstructions.IntervalTermination)
                {
                    timeSpanList.Add(
                        new TickStrategy
                        {
                            TickOffset = initialTick,
                            Strategy = strategy
                        });

                    initialTick = initialTick.Add(subPlan.EquityInstructions.UpdateFrequency);
                }
            }

            timeSpanList = timeSpanList.OrderBy(i => i.TickOffset).ToList();
            foreach (var item in timeSpanList)
            {
                if (item?.Strategy == null)
                {
                    continue;
                }

                Tick(frame.Epoch.Date.Add(item.TickOffset), item.Strategy);
            }
        }

        private List<TickStrategy> RemoveConflictingTicks(List<TickStrategy> timeSpanList)
        {
            if (_plan == null)
            {
                return timeSpanList;
            }

            foreach (var subPlan in _plan.Where(pla => pla != null))
            {
                var itemsToEliminate = timeSpanList
                    .Where(tpl =>
                        tpl.TickOffset >= subPlan.EquityInstructions.IntervalCommencement
                        && tpl.TickOffset <= subPlan.EquityInstructions.IntervalTermination)
                    .ToList();

                timeSpanList = timeSpanList.Except(itemsToEliminate).ToList();
            }

            return timeSpanList;
        }

        private void Tick(DateTime advanceTick, IEquityDataGeneratorStrategy strategy)
        {
            lock (_walkingLock)
            {
                var tockedSecurities = _activeFrame
                    .Securities
                    .Select(sec => strategy.AdvanceFrame(sec, advanceTick, false))
                    .ToArray();

                var tickTock = new EquityIntraDayTimeBarCollection(_activeFrame.Exchange, advanceTick, tockedSecurities);
                _activeFrame = tickTock;

                _stream.Add(tickTock);
            }
        }

        public void TerminateWalk()
        {
            lock (_stateTransitionLock)
            {
                _logger.LogInformation("Random walk generator terminating walk");
            }
        }

        private class TickStrategy
        {
            public TimeSpan TickOffset { get; set; }
            public IEquityDataGeneratorStrategy Strategy { get; set; }
        }
    }
}
