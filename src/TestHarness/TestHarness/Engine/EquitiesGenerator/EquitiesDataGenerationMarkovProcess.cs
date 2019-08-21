namespace TestHarness.Engine.EquitiesGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Markets.Collections;
    using Domain.Surveillance.Streams.Interfaces;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService.Api.Markets;
    using RedDeer.Contracts.SurveillanceService.Api.SecurityPrices;

    using TestHarness.Engine.EquitiesGenerator.Interfaces;
    using TestHarness.Engine.EquitiesGenerator.Strategies;
    using TestHarness.Engine.EquitiesGenerator.Strategies.Interfaces;
    using TestHarness.Engine.Plans;

    /// <summary>
    ///     Apply a random walk to securities in your stream
    ///     Not multi thread safe, only use with transitory life style
    /// </summary>
    public class EquitiesDataGenerationMarkovProcess : IEquitiesDataGenerationMarkovProcess
    {
        private readonly IEquityDataGeneratorStrategy _dataStrategy;

        private readonly ILogger _logger;

        private readonly IReadOnlyCollection<DataGenerationPlan> _plan;

        private readonly object _stateTransitionLock = new object();

        private readonly TimeSpan _tickSeparation;

        private readonly object _walkingLock = new object();

        private EquityIntraDayTimeBarCollection _activeFrame;

        private IStockExchangeStream _stream;

        public EquitiesDataGenerationMarkovProcess(
            IEquityDataGeneratorStrategy dataStrategy,
            IReadOnlyCollection<DataGenerationPlan> plan,
            TimeSpan tickSeparation,
            ILogger logger)
        {
            this._dataStrategy = dataStrategy ?? throw new ArgumentNullException(nameof(dataStrategy));

            this._tickSeparation = tickSeparation;
            this._plan = plan ?? new List<DataGenerationPlan>();

            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void InitiateWalk(IStockExchangeStream stream, ExchangeDto market, SecurityPriceResponseDto prices)
        {
            this._logger.LogInformation("Walk initiated in equity generator");

            if (stream == null) throw new ArgumentNullException(nameof(stream));

            if (market == null) throw new ArgumentNullException(nameof(market));

            if (prices == null) throw new ArgumentNullException(nameof(prices));

            lock (this._stateTransitionLock)
            {
                this._stream = stream;

                var apiGeneration = new ApiDataGenerationInitialiser(
                    market,
                    this._tickSeparation,
                    prices.SecurityPrices);
                var framesToGenerateFor = apiGeneration.OrderedDailyFrames();

                foreach (var frame in framesToGenerateFor)
                {
                    this._activeFrame = frame;
                    this.AdvanceFrames(market, frame);
                }
            }
        }

        public void TerminateWalk()
        {
            lock (this._stateTransitionLock)
            {
                this._logger.LogInformation("Random walk generator terminating walk");
            }
        }

        private void AdvanceFrames(ExchangeDto market, EquityIntraDayTimeBarCollection frame)
        {
            if (frame.Epoch.TimeOfDay > market.MarketCloseTime)
            {
                this._logger.Log(LogLevel.Error, "ended up advancing a frame whose start exceeded market close time");
                return;
            }

            var timeSpanList = new List<TickStrategy>();
            var advanceTick = frame.Epoch.TimeOfDay;

            while (advanceTick <= market.MarketCloseTime)
            {
                timeSpanList.Add(new TickStrategy { TickOffset = advanceTick, Strategy = this._dataStrategy });

                advanceTick = advanceTick.Add(this._tickSeparation);
            }

            timeSpanList = this.RemoveConflictingTicks(timeSpanList);

            foreach (var subPlan in this._plan)
            {
                var strategy = new PlanEquityStrategy(subPlan, this._dataStrategy);
                var initialTick = subPlan.EquityInstructions.IntervalCommencement;
                while (initialTick <= subPlan.EquityInstructions.IntervalTermination)
                {
                    timeSpanList.Add(new TickStrategy { TickOffset = initialTick, Strategy = strategy });

                    initialTick = initialTick.Add(subPlan.EquityInstructions.UpdateFrequency);
                }
            }

            timeSpanList = timeSpanList.OrderBy(i => i.TickOffset).ToList();
            foreach (var item in timeSpanList)
            {
                if (item?.Strategy == null) continue;

                this.Tick(frame.Epoch.Date.Add(item.TickOffset), item.Strategy);
            }
        }

        private List<TickStrategy> RemoveConflictingTicks(List<TickStrategy> timeSpanList)
        {
            if (this._plan == null) return timeSpanList;

            foreach (var subPlan in this._plan.Where(pla => pla != null))
            {
                var itemsToEliminate = timeSpanList.Where(
                    tpl => tpl.TickOffset >= subPlan.EquityInstructions.IntervalCommencement
                           && tpl.TickOffset <= subPlan.EquityInstructions.IntervalTermination).ToList();

                timeSpanList = timeSpanList.Except(itemsToEliminate).ToList();
            }

            return timeSpanList;
        }

        private void Tick(DateTime advanceTick, IEquityDataGeneratorStrategy strategy)
        {
            lock (this._walkingLock)
            {
                var tockedSecurities = this._activeFrame.Securities
                    .Select(sec => strategy.AdvanceFrame(sec, advanceTick, false)).ToArray();

                var tickTock = new EquityIntraDayTimeBarCollection(
                    this._activeFrame.Exchange,
                    advanceTick,
                    tockedSecurities);
                this._activeFrame = tickTock;

                this._stream.Add(tickTock);
            }
        }

        private class TickStrategy
        {
            public IEquityDataGeneratorStrategy Strategy { get; set; }

            public TimeSpan TickOffset { get; set; }
        }
    }
}