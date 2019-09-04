using System;
using Domain.Core.Financial.Money;
using Domain.Core.Markets.Timebars;
using TestHarness.Engine.EquitiesGenerator.Strategies.Interfaces;
using TestHarness.Engine.Plans;

namespace TestHarness.Engine.EquitiesGenerator.Strategies
{
    public class PlanEquityStrategy : IEquityDataGeneratorStrategy
    {
        private readonly DataGenerationPlan _plan;
        private readonly IEquityDataGeneratorStrategy _baseStrategy;

        public PlanEquityStrategy(
            DataGenerationPlan plan,
            IEquityDataGeneratorStrategy baseStrategy)
        {
            _plan = plan ?? throw new ArgumentNullException(nameof(plan));
            _baseStrategy = baseStrategy ?? throw new ArgumentNullException(nameof(baseStrategy));
        }

        public EquityInstrumentIntraDayTimeBar AdvanceFrame(
            EquityInstrumentIntraDayTimeBar tick,
            DateTime advanceTick,
            bool walkIntraday)
        {
            if (!string.Equals(tick.Security.Identifiers.Sedol, _plan.Sedol, StringComparison.InvariantCultureIgnoreCase))
            {
               return _baseStrategy.AdvanceFrame(tick, advanceTick, walkIntraday);
            }

            var initialAdvancedFrame = _baseStrategy.AdvanceFrame(tick, advanceTick, walkIntraday);

            return AdjustToPlan(initialAdvancedFrame, tick);
        }

        private EquityInstrumentIntraDayTimeBar AdjustToPlan(EquityInstrumentIntraDayTimeBar tick, EquityInstrumentIntraDayTimeBar precedingTick)
        {
            var adjustedSpread = AdjustedSpread(tick, precedingTick);

            return new EquityInstrumentIntraDayTimeBar(
                tick.Security,
                adjustedSpread,
                AdjustedDailies(tick),
                tick.TimeStamp,
                tick.Market);
        }

        private DailySummaryTimeBar AdjustedDailies(EquityInstrumentIntraDayTimeBar tick)
        {
            return new DailySummaryTimeBar(
                tick.DailySummaryTimeBar.MarketCap.Value.Value,
                tick.DailySummaryTimeBar.MarketCap.Value.Currency.Code,
                tick.DailySummaryTimeBar.IntradayPrices,
                tick.DailySummaryTimeBar.ListedSecurities,
                tick.DailySummaryTimeBar.DailyVolume,
                tick.TimeStamp);
        }

        private SpreadTimeBar AdjustedSpread(EquityInstrumentIntraDayTimeBar tick, EquityInstrumentIntraDayTimeBar precedingTick)
        {
            switch (_plan.EquityInstructions.PriceManipulation)
            {
                case PriceManipulation.Consistent:
                    return precedingTick.SpreadTimeBar;
                case PriceManipulation.Random:
                    return tick.SpreadTimeBar;
                case PriceManipulation.Increase:
                    return
                        AdjustSpreadCalculation(
                            _plan.EquityInstructions.PriceTickDelta.GetValueOrDefault(0) + 1,
                            precedingTick,
                            tick);
                case PriceManipulation.Decrease:
                    return
                        AdjustSpreadCalculation(
                            1 - _plan.EquityInstructions.PriceTickDelta.GetValueOrDefault(0),
                            precedingTick,
                            tick);
            }

            return tick.SpreadTimeBar;
        }

        private SpreadTimeBar AdjustSpreadCalculation(decimal adjustmentFactor, EquityInstrumentIntraDayTimeBar precedingTick, EquityInstrumentIntraDayTimeBar tick)
        {
            var adjustedBid =
                new Money(precedingTick.SpreadTimeBar.Bid.Value * adjustmentFactor, precedingTick.SpreadTimeBar.Bid.Currency);
            var adjustedAsk =
                new Money(precedingTick.SpreadTimeBar.Ask.Value * adjustmentFactor, precedingTick.SpreadTimeBar.Ask.Currency);
            var adjustedPrice =
                new Money(precedingTick.SpreadTimeBar.Price.Value * adjustmentFactor, precedingTick.SpreadTimeBar.Price.Currency);

            return new SpreadTimeBar(adjustedBid, adjustedAsk, adjustedPrice, tick.SpreadTimeBar.Volume);
        }

        public EquityGenerationStrategies Strategy { get; } = EquityGenerationStrategies.Plan;
    }
}
