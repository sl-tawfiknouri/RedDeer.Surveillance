namespace TestHarness.Engine.EquitiesGenerator.Strategies
{
    using System;

    using Domain.Core.Financial.Money;
    using Domain.Core.Markets.Timebars;

    using TestHarness.Engine.EquitiesGenerator.Strategies.Interfaces;
    using TestHarness.Engine.Plans;

    public class PlanEquityStrategy : IEquityDataGeneratorStrategy
    {
        private readonly IEquityDataGeneratorStrategy _baseStrategy;

        private readonly DataGenerationPlan _plan;

        public PlanEquityStrategy(DataGenerationPlan plan, IEquityDataGeneratorStrategy baseStrategy)
        {
            this._plan = plan ?? throw new ArgumentNullException(nameof(plan));
            this._baseStrategy = baseStrategy ?? throw new ArgumentNullException(nameof(baseStrategy));
        }

        public EquityGenerationStrategies Strategy { get; } = EquityGenerationStrategies.Plan;

        public EquityInstrumentIntraDayTimeBar AdvanceFrame(
            EquityInstrumentIntraDayTimeBar tick,
            DateTime advanceTick,
            bool walkIntraday)
        {
            if (!string.Equals(
                    tick.Security.Identifiers.Sedol,
                    this._plan.Sedol,
                    StringComparison.InvariantCultureIgnoreCase))
                return this._baseStrategy.AdvanceFrame(tick, advanceTick, walkIntraday);

            var initialAdvancedFrame = this._baseStrategy.AdvanceFrame(tick, advanceTick, walkIntraday);

            return this.AdjustToPlan(initialAdvancedFrame, tick);
        }

        private DailySummaryTimeBar AdjustedDailies(EquityInstrumentIntraDayTimeBar tick)
        {
            return new DailySummaryTimeBar(
                tick.DailySummaryTimeBar.MarketCap,
                tick.DailySummaryTimeBar.IntradayPrices,
                tick.DailySummaryTimeBar.ListedSecurities,
                tick.DailySummaryTimeBar.DailyVolume,
                tick.TimeStamp);
        }

        private SpreadTimeBar AdjustedSpread(
            EquityInstrumentIntraDayTimeBar tick,
            EquityInstrumentIntraDayTimeBar precedingTick)
        {
            switch (this._plan.EquityInstructions.PriceManipulation)
            {
                case PriceManipulation.Consistent:
                    return precedingTick.SpreadTimeBar;
                case PriceManipulation.Random:
                    return tick.SpreadTimeBar;
                case PriceManipulation.Increase:
                    return this.AdjustSpreadCalculation(
                        this._plan.EquityInstructions.PriceTickDelta.GetValueOrDefault(0) + 1,
                        precedingTick,
                        tick);
                case PriceManipulation.Decrease:
                    return this.AdjustSpreadCalculation(
                        1 - this._plan.EquityInstructions.PriceTickDelta.GetValueOrDefault(0),
                        precedingTick,
                        tick);
            }

            return tick.SpreadTimeBar;
        }

        private SpreadTimeBar AdjustSpreadCalculation(
            decimal adjustmentFactor,
            EquityInstrumentIntraDayTimeBar precedingTick,
            EquityInstrumentIntraDayTimeBar tick)
        {
            var adjustedBid = new Money(
                precedingTick.SpreadTimeBar.Bid.Value * adjustmentFactor,
                precedingTick.SpreadTimeBar.Bid.Currency);
            var adjustedAsk = new Money(
                precedingTick.SpreadTimeBar.Ask.Value * adjustmentFactor,
                precedingTick.SpreadTimeBar.Ask.Currency);
            var adjustedPrice = new Money(
                precedingTick.SpreadTimeBar.Price.Value * adjustmentFactor,
                precedingTick.SpreadTimeBar.Price.Currency);

            return new SpreadTimeBar(adjustedBid, adjustedAsk, adjustedPrice, tick.SpreadTimeBar.Volume);
        }

        private EquityInstrumentIntraDayTimeBar AdjustToPlan(
            EquityInstrumentIntraDayTimeBar tick,
            EquityInstrumentIntraDayTimeBar precedingTick)
        {
            var adjustedSpread = this.AdjustedSpread(tick, precedingTick);

            return new EquityInstrumentIntraDayTimeBar(
                tick.Security,
                adjustedSpread,
                this.AdjustedDailies(tick),
                tick.TimeStamp,
                tick.Market);
        }
    }
}