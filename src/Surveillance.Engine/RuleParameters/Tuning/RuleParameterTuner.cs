using System;
using System.Collections.Generic;
using System.Linq;
using Accord.IO;
using Surveillance.Engine.Rules.RuleParameters.Equities;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Tuning
{
    public class RuleParameterTuner
    {
        public IReadOnlyCollection<ICancelledOrderRuleEquitiesParameters> Parameters(
            ICancelledOrderRuleEquitiesParameters parameters)
        {
            if (parameters == null)
            {
                return new List<ICancelledOrderRuleEquitiesParameters>();
            }

            var cancelledOrderCountPercentageTuning =
                PermutateDecimal(
                    parameters.CancelledOrderCountPercentageThreshold,
                    nameof(parameters.CancelledOrderCountPercentageThreshold));

            var cancelledOrderCountPercentageTuningProjections =
                cancelledOrderCountPercentageTuning
                    .Select((_, x) =>
                    {
                        var clone = (CancelledOrderRuleEquitiesParameters)parameters.DeepClone();
                        clone.Id = TunedId(parameters.Id, nameof(parameters.CancelledOrderCountPercentageThreshold), x);
                        clone.CancelledOrderCountPercentageThreshold = _.TunedValue;

                        return clone;
                    })
                    .ToList();

            var cancelledOrderPositionPercentageTuning =
                PermutateDecimal(
                    parameters.CancelledOrderPercentagePositionThreshold,
                    nameof(parameters.CancelledOrderPercentagePositionThreshold));

            var cancelledOrderPositionPercentageTuningProjections =
                cancelledOrderPositionPercentageTuning
                    .Select((_,x) =>
                    {
                        var clone = (CancelledOrderRuleEquitiesParameters)parameters.DeepClone();
                        clone.Id = TunedId(parameters.Id, nameof(parameters.CancelledOrderCountPercentageThreshold), x);
                        clone.CancelledOrderPercentagePositionThreshold = _.TunedValue;

                        return clone;
                    })
                    .ToList();

            var cancelledOrderWindowTuning =
                PermutateTimeWindows(
                    parameters.WindowSize,
                    nameof(parameters.WindowSize));

            var cancelledOrderWindowTuningProjections =
                cancelledOrderWindowTuning
                    .Select((_, x) =>
                    {
                        var clone = (CancelledOrderRuleEquitiesParameters)parameters.DeepClone();
                        clone.Id = TunedId(parameters.Id, nameof(parameters.WindowSize), x);
                        clone.WindowSize = _.TunedValue;

                        return clone;
                    })
                    .ToList();

            var cancelledOrderMinimumTradesProjections =
                PermutateInteger(
                    parameters.MinimumNumberOfTradesToApplyRuleTo,
                    nameof(parameters.MinimumNumberOfTradesToApplyRuleTo),
                    2);

            var cancelledOrderMinimumTradesTuningProjections =
                cancelledOrderMinimumTradesProjections
                    .Select((_, x) =>
                    {
                        var clone = (CancelledOrderRuleEquitiesParameters)parameters.DeepClone();
                        clone.Id = TunedId(parameters.Id, nameof(parameters.MinimumNumberOfTradesToApplyRuleTo), x);
                        clone.MinimumNumberOfTradesToApplyRuleTo = _.TunedValue;

                        return clone;
                    })
                    .ToList();

            var cancelledOrderMaximumTradesProjections =
                PermutateInteger(
                    parameters.MaximumNumberOfTradesToApplyRuleTo,
                    nameof(parameters.MaximumNumberOfTradesToApplyRuleTo),
                    4);

            var cancelledOrderMaximumTradesTuningProjections =
                cancelledOrderMaximumTradesProjections
                    .Select((_, x) =>
                    {
                        var clone = (CancelledOrderRuleEquitiesParameters)parameters.DeepClone();
                        clone.Id = TunedId(parameters.Id, nameof(parameters.MaximumNumberOfTradesToApplyRuleTo), x);
                        clone.MaximumNumberOfTradesToApplyRuleTo = _.TunedValue;

                        return clone;
                    })
                    .ToList();

            return
                new List<ICancelledOrderRuleEquitiesParameters>()
                    .Concat(cancelledOrderCountPercentageTuningProjections)
                    .Concat(cancelledOrderPositionPercentageTuningProjections)
                    .Concat(cancelledOrderWindowTuningProjections)
                    .Concat(cancelledOrderMinimumTradesTuningProjections)
                    .Concat(cancelledOrderMaximumTradesTuningProjections)
                    .ToList();
        }

        private string TunedId(string raw, string property, int id)
        {
            return $"{raw}-tuned-{property}-{id}";
        }

        private IReadOnlyCollection<TunedParameter<decimal>> PermutateDecimal(decimal? input, string name)
        {
            if (!input.HasValue)
            {
                return new TunedParameter<decimal>[0];
            }

            if (input < 0)
            {
                return new TunedParameter<decimal>[0];
            }

            var smallOffset = 0.1m;
            var mediumOffset = 0.25m;
            var largeOffset = 0.5m;

            var appliedSmallOffset = input.GetValueOrDefault() * smallOffset;
            var appliedMediumOffset = input.GetValueOrDefault() * mediumOffset;
            var appliedLargeOffset = input.GetValueOrDefault() * largeOffset;

            var smallOffsets = ApplyDecimalOffset(input.GetValueOrDefault(), appliedSmallOffset, name);
            var mediumOffsets = ApplyDecimalOffset(input.GetValueOrDefault(), appliedMediumOffset, name);
            var largeOffsets = ApplyDecimalOffset(input.GetValueOrDefault(), appliedLargeOffset, name);

            return smallOffsets.Concat(mediumOffsets).Concat(largeOffsets).Distinct().ToList();
        }

        private TunedParameter<decimal>[] ApplyDecimalOffset(decimal value, decimal offset, string name)
        {
            var positiveOffset = new TunedParameter<decimal>(value, Math.Min(value + offset, 1), name);
            var negativeOffset = new TunedParameter<decimal>(value, Math.Max(value - offset, 0), name);

            return new[] { positiveOffset, negativeOffset };
        }

        private IReadOnlyCollection<TunedParameter<int>> PermutateInteger(int? input, string name, int? min)
        {
            if (input == null)
            {
                return new TunedParameter<int>[0];
            }

            if (input < 0)
            {
                return new TunedParameter<int>[0];
            }

            var smallOffset = 0.1m;
            var mediumOffset = 1.5;
            var largeOffset = 3;

            var appliedSmallOffset = (int)Math.Max(input.Value * smallOffset, min.GetValueOrDefault());
            var appliedMediumOffset = (int)(input.Value * mediumOffset);
            var appliedLargeOffset = (int)(input.Value * largeOffset);
            
            var smallOffsets = ApplyIntegerOffset(input.Value, appliedSmallOffset, name, min);
            var mediumOffsets = ApplyIntegerOffset(input.Value, appliedMediumOffset, name, min);
            var largeOffsets = ApplyIntegerOffset(input.Value, appliedLargeOffset, name, min);

            return smallOffsets.Concat(mediumOffsets).Concat(largeOffsets).Distinct().ToList();
        }

        private TunedParameter<int>[] ApplyIntegerOffset(int value, int offset, string name, int? min)
        {
            var positiveOffset = new TunedParameter<int>(value, value + offset, name);
            var negativeOffset = new TunedParameter<int>(value, Math.Max(value - offset, min.GetValueOrDefault(0)), name);

            return new[] { positiveOffset, negativeOffset };
        }

        private IReadOnlyCollection<TunedParameter<TimeSpan>> PermutateTimeWindows(TimeSpan input, string name)
        {
            var smallOffset = 0.1m;
            var mediumOffset = 1.5m;
            var largeOffset = 3m;

            var appliedSmallOffset = TimeSpan.FromTicks((long)(input.Ticks * smallOffset));
            var appliedMediumOffset = TimeSpan.FromTicks((long)(input.Ticks * mediumOffset));
            var appliedLargeOffset = TimeSpan.FromTicks((long)(input.Ticks * largeOffset));

            var smallOffsets = ApplyTimeSpanOffset(input, appliedSmallOffset, name);
            var mediumOffsets = ApplyTimeSpanOffset(input, appliedMediumOffset, name);
            var largeOffsets = ApplyTimeSpanOffset(input, appliedLargeOffset, name);

            return (new[]
            {
                new TunedParameter<TimeSpan>(input, TimeSpan.FromHours(1), name),
                new TunedParameter<TimeSpan>(input, TimeSpan.FromDays(1), name),
                new TunedParameter<TimeSpan>(input, TimeSpan.FromDays(7), name),
                new TunedParameter<TimeSpan>(input, TimeSpan.FromDays(14), name),
                new TunedParameter<TimeSpan>(input, TimeSpan.FromDays(28), name)
            })
                .Concat(smallOffsets)
                .Concat(mediumOffsets)
                .Concat(largeOffsets)
                .ToList();
        }

        private TunedParameter<TimeSpan>[] ApplyTimeSpanOffset(TimeSpan value, TimeSpan offset, string name)
        {
            var positiveOffset = new TunedParameter<TimeSpan>(value, value + offset, name);
            if (value < offset)
            {
                return new[] { positiveOffset };
            }

            var negativeOffset = new TunedParameter<TimeSpan>(value, value - offset, name);

            return new[] { positiveOffset, negativeOffset };
        }
    }
}
