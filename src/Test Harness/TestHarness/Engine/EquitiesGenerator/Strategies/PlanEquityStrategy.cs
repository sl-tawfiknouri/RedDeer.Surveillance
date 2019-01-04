using System;
using DomainV2.Equity;
using DomainV2.Equity.TimeBars;
using DomainV2.Financial;
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

        public FinancialInstrumentTimeBar AdvanceFrame(
            FinancialInstrumentTimeBar tick,
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

        private FinancialInstrumentTimeBar AdjustToPlan(FinancialInstrumentTimeBar tick, FinancialInstrumentTimeBar precedingTick)
        {
            var adjustedSpread = AdjustedSpread(tick, precedingTick);

            return new FinancialInstrumentTimeBar(
                tick.Security,
                adjustedSpread,
                tick.Volume,
                tick.DailyVolume,
                tick.TimeStamp,
                tick.MarketCap,
                tick.IntradayPrices,
                tick.ListedSecurities,
                tick.Market);
        }

        private Spread AdjustedSpread(FinancialInstrumentTimeBar tick, FinancialInstrumentTimeBar precedingTick)
        {
            switch (_plan.EquityInstructions.PriceManipulation)
            {
                case PriceManipulation.Consistent:
                    return precedingTick.Spread;
                case PriceManipulation.Random:
                    return tick.Spread;
                case PriceManipulation.Increase:
                    return
                        AdjustSpreadCalculation(
                            _plan.EquityInstructions.PriceTickDelta.GetValueOrDefault(0) + 1,
                            precedingTick);
                case PriceManipulation.Decrease:
                    return
                        AdjustSpreadCalculation(
                            1 - _plan.EquityInstructions.PriceTickDelta.GetValueOrDefault(0),
                            precedingTick);
            }

            return tick.Spread;
        }

        private Spread AdjustSpreadCalculation(decimal adjustmentFactor, FinancialInstrumentTimeBar precedingTick)
        {
            var adjustedBid =
                new CurrencyAmount(precedingTick.Spread.Bid.Value * adjustmentFactor, precedingTick.Spread.Bid.Currency);
            var adjustedAsk =
                new CurrencyAmount(precedingTick.Spread.Ask.Value * adjustmentFactor, precedingTick.Spread.Ask.Currency);
            var adjustedPrice =
                new CurrencyAmount(precedingTick.Spread.Price.Value * adjustmentFactor, precedingTick.Spread.Price.Currency);

            return new Spread(adjustedBid, adjustedAsk, adjustedPrice);
        }

        public EquityGenerationStrategies Strategy { get; } = EquityGenerationStrategies.Plan;
    }
}
