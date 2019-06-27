using System;
using System.Collections.Generic;
using System.Linq;
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
                        return new CancelledOrderRuleEquitiesParameters(
                            TunedId(parameters.Id, nameof(parameters.CancelledOrderCountPercentageThreshold), x),
                            parameters.WindowSize,
                            _.TunedValue,
                            parameters.CancelledOrderPercentagePositionThreshold,
                            parameters.MinimumNumberOfTradesToApplyRuleTo,
                            parameters.MaximumNumberOfTradesToApplyRuleTo,
                            parameters.Factors,
                            parameters.AggregateNonFactorableIntoOwnCategory);
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
                        return new CancelledOrderRuleEquitiesParameters(
                            TunedId(parameters.Id, nameof(parameters.CancelledOrderPercentagePositionThreshold), x),
                            parameters.WindowSize,
                            parameters.CancelledOrderCountPercentageThreshold,
                            _.TunedValue,
                            parameters.MinimumNumberOfTradesToApplyRuleTo,
                            parameters.MaximumNumberOfTradesToApplyRuleTo,
                            parameters.Factors,
                            parameters.AggregateNonFactorableIntoOwnCategory);
                    })
                    .ToList();

            return
                new List<ICancelledOrderRuleEquitiesParameters>()
                    .Concat(cancelledOrderCountPercentageTuningProjections)
                    .Concat(cancelledOrderPositionPercentageTuningProjections)
                    .ToList();
        }

        private string TunedId(string raw, string property, int id)
        {
            return $"{raw}-tuned-{property}-{id}";
        }

        private IReadOnlyCollection<TunedParameter<decimal?>> PermutateDecimal(decimal? input, string name)
        {
            if (!input.HasValue)
            {
                return new TunedParameter<decimal?>[0];
            }

            if (input == 0)
            {
                return new TunedParameter<decimal?>[0];
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

            return smallOffsets.Concat(mediumOffsets).Concat(largeOffsets).ToList();
        }

        private TunedParameter<decimal?>[] ApplyDecimalOffset(decimal value, decimal offset, string name)
        {
            var positiveOffset = new TunedParameter<decimal?>(value, Math.Min(value + offset, 1), name);
            var negativeOffset = new TunedParameter<decimal?>(value, Math.Max(value - offset, 0), name);

            return new[] { positiveOffset, negativeOffset };
        }
    }
}
