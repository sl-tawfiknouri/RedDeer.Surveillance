﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Accord.IO;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.RuleParameters.Tuning.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Tuning
{
    public class RuleParameterTuner : IRuleParameterTuner
    {
        private readonly ILogger<RuleParameterTuner> _logger;

        public RuleParameterTuner(ILogger<RuleParameterTuner> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyCollection<T> ParametersFramework<T>(T parameters) where T : class
        {
            if (parameters == null)
            {
                _logger.LogInformation($"Received a null parameters argument. Returning");
                return new List<T>();
            }

            var hasSerializable = parameters.GetType().IsDefined(typeof(SerializableAttribute), true);

            if (!hasSerializable)
            {
                _logger.LogInformation($"type {typeof(T)} was passed to the rule parameter tuner without a serializable attribute defined");
                return new List<T>();
            }

            var props = parameters.GetType().GetProperties() ?? new PropertyInfo[0];
            var idField = props.FirstOrDefault(_ => Attribute.IsDefined(_, typeof(TuneableIdParameter)));

            if (idField == null)
            {
                _logger.LogError($"Could not identify an id field on {nameof(parameters)}");
                return new List<T>();
            }

            var boolAttributes = props.Where(_ => Attribute.IsDefined(_, typeof(TuneableBoolParameter))).ToList();
            var decimalAttributes = props.Where(_ => Attribute.IsDefined(_, typeof(TuneableDecimalParameter))).ToList();
            var integerAttributes = props.Where(_ => Attribute.IsDefined(_, typeof(TuneableIntegerParameter))).ToList();
            var timespanAttributes = props.Where(_ => Attribute.IsDefined(_, typeof(TuneableTimespanParameter))).ToList();
            var timeWindowAttributes = props.Where(_ => Attribute.IsDefined(_, typeof(TuneableTimeWindowParameter))).ToList();
            var tunedParamAttributes = props.FirstOrDefault(_ => Attribute.IsDefined(_, typeof(TunedParam)));

            var boolPermutations =
                boolAttributes
                    .Select(_ => PermutateBoolAttribute(_, parameters.DeepClone(), idField, tunedParamAttributes))
                    .SelectMany(_ => _)
                    .Select(_ => _ as T)
                    .ToList();

            var decimalPermutations =
                 decimalAttributes
                     .Select(_ => PermutateDecimalAttribute(_, parameters.DeepClone(), idField, tunedParamAttributes))
                     .SelectMany(_ => _)
                     .Select(_ => _ as T)
                     .ToList();

            var integerPermutations =
                integerAttributes
                    .Select(_ => PermutateIntegerAttribute(_, parameters.DeepClone(), idField, tunedParamAttributes))
                    .SelectMany(_ => _)
                    .Select(_ => _ as T)
                    .ToList();

            var timespanPermutations =
                timespanAttributes
                    .Select(_ => PermutateTimespanAttribute(_, parameters.DeepClone(), idField, tunedParamAttributes))
                    .SelectMany(_ => _)
                    .Select(_ => _ as T)
                    .ToList();

            var timeWindowPermutations =
                timeWindowAttributes
                    .Select(_ => PermutateTimeWindowAttribute(_, parameters.DeepClone(), tunedParamAttributes))
                    .SelectMany(_ => _)
                    .Select(_ => _ as T)
                    .ToList();

            return
                decimalPermutations
                    .Concat(boolPermutations)
                    .Concat(integerPermutations)
                    .Concat(timespanPermutations)
                    .Concat(timeWindowPermutations)
                    .Where(_ => _ != null)
                    .ToList();
        }

        private List<object> PermutateDecimalAttribute(PropertyInfo pInfo, object target, PropertyInfo idInfo, PropertyInfo tunableRule)
        {
            if (!pInfo.CanWrite)
            {
                _logger.LogInformation($"Received a unwritable property parameters argument {pInfo.Name}. Returning");
                return null;
            }

            var baseId = idInfo.GetMethod.Invoke(target, new object[0]) as string;
            var baseDecimalValue = pInfo.GetMethod.Invoke(target, new object[0]) as decimal?;
            var tuningPermutations = PermutateDecimal(baseDecimalValue, pInfo.Name);

            var projectedTunedParameters =
                tuningPermutations
                    .Select((_, x) =>
                    {
                        var clone = target.DeepClone();
                        var tuningId = TunedId(baseId, pInfo.Name, x);
                        idInfo.SetMethod.Invoke(clone, new[] { (object)tuningId });
                        pInfo.SetMethod.Invoke(clone, new[] { (object)_.TunedValue });
                        if (tunableRule != null)
                            tunableRule.SetMethod.Invoke(clone, new[] { (object)_.MapToString() });
                        return clone;
                    })
                    .ToList();

            return projectedTunedParameters;
        }

        private List<object> PermutateIntegerAttribute(PropertyInfo pInfo, object target, PropertyInfo idInfo, PropertyInfo tunableRule)
        {
            if (!pInfo.CanWrite)
            {
                _logger.LogInformation($"Received a unwritable property parameters argument {pInfo.Name}. Returning");
                return null;
            }

            var baseId = idInfo.GetMethod.Invoke(target, new object[0]) as string;
            var baseIntegerValue = pInfo.GetMethod.Invoke(target, new object[0]) as int?;
            var tuningPermutations = PermutateInteger(baseIntegerValue, pInfo.Name, null);

            var projectedTunedParameters =
                tuningPermutations
                    .Select((_, x) =>
                    {
                        var clone = target.DeepClone();
                        var tuningId = TunedId(baseId, pInfo.Name, x);
                        idInfo.SetMethod.Invoke(clone, new[] { (object)tuningId });
                        pInfo.SetMethod.Invoke(clone, new[] { (object)_.TunedValue });
                        if (tunableRule != null)
                            tunableRule.SetMethod.Invoke(clone, new[] { (object)_.MapToString() });
                        return clone;
                    })
                    .ToList();

            return projectedTunedParameters;
        }

        private List<object> PermutateTimespanAttribute(PropertyInfo pInfo, object target, PropertyInfo idInfo, PropertyInfo tunableRule)
        {
            if (!pInfo.CanWrite)
            {
                _logger.LogInformation($"Received a unwritable property parameters argument {pInfo.Name}. Returning");
                return null;
            }

            var baseId = idInfo.GetMethod.Invoke(target, new object[0]) as string;
            var baseTimespanValue = pInfo.GetMethod.Invoke(target, new object[0]) as TimeSpan?;
            var tuningPermutations = PermutateTimeSpans(baseTimespanValue, pInfo.Name);

            var projectedTunedParameters =
                tuningPermutations
                    .Select((_, x) =>
                    {
                        var clone = target.DeepClone();
                        var tuningId = TunedId(baseId, pInfo.Name, x);
                        idInfo.SetMethod.Invoke(clone, new[] { (object)tuningId });
                        pInfo.SetMethod.Invoke(clone, new[] { (object)_.TunedValue });
                        if (tunableRule != null)
                            tunableRule.SetMethod.Invoke(clone, new[] { (object)_.MapToString() });
                        return clone;
                    })
                    .ToList();

            return projectedTunedParameters;
        }

        private List<object> PermutateTimeWindowAttribute(PropertyInfo pInfo, object target, PropertyInfo tunableRule)
        {
            if (!pInfo.CanWrite)
            {
                _logger.LogInformation($"Received a unwritable property parameters argument {pInfo.Name}. Returning");
                return null;
            }

            var timeWindowObj = pInfo.GetMethod.Invoke(target, new object[0]) as TimeWindows;
            var tunedWindow = ParametersFramework(timeWindowObj);

            var tunedTimeWindows = tunedWindow.Select(_ => ApplyTimeWindow(pInfo, target, _, tunableRule)).ToList();

            return tunedTimeWindows;
        }

        private object ApplyTimeWindow(PropertyInfo pInfo, object targetBackward, TimeWindows timeWindow, PropertyInfo tunableRule)
        {
            var clonedTarget = targetBackward.DeepClone();
            pInfo.SetMethod.Invoke(clonedTarget, new [] { timeWindow });

            var props = timeWindow.GetType().GetProperties() ?? new PropertyInfo[0];
            var tunedParamAttributes = props.FirstOrDefault(_ => Attribute.IsDefined(_, typeof(TunedParam)));

            if (tunedParamAttributes == null)
            {
                return clonedTarget;
            }

            var tunedParam = tunedParamAttributes.GetMethod.Invoke(timeWindow, new object[0]);
            tunableRule.SetMethod.Invoke(clonedTarget, new[] { tunedParam });

            return clonedTarget;
        }

        private List<object> PermutateBoolAttribute(PropertyInfo pInfo, object target, PropertyInfo idInfo, PropertyInfo tunableRule)
        {
            if (!pInfo.CanWrite)
            {
                _logger.LogInformation($"Received a unwritable property parameters argument {pInfo.Name}. Returning");
                return null;
            }

            var baseId = idInfo.GetMethod.Invoke(target, new object[0]) as string;
            var baseBoolValue = pInfo.GetMethod.Invoke(target, new object[0]) as bool?;
            var tuningPermutations = PermutateBool(baseBoolValue, pInfo.Name);

            var projectedTunedParameters =
                tuningPermutations
                    .Select((_, x) =>
                    {
                        var clone = target.DeepClone();
                        var tuningId = TunedId(baseId, pInfo.Name, x);
                        idInfo.SetMethod.Invoke(clone, new[] { (object)tuningId });
                        pInfo.SetMethod.Invoke(clone, new[] { (object)_.TunedValue });
                        if (tunableRule != null)
                            tunableRule.SetMethod.Invoke(clone, new[] { (object)_.MapToString() });
                        return clone;
                    })
                    .ToList();

            return projectedTunedParameters;
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

            var smallOffsets = ApplyDecimalOffset(input.GetValueOrDefault(), appliedSmallOffset, name, TuningStrength.Small);
            var mediumOffsets = ApplyDecimalOffset(input.GetValueOrDefault(), appliedMediumOffset, name, TuningStrength.Medium);
            var largeOffsets = ApplyDecimalOffset(input.GetValueOrDefault(), appliedLargeOffset, name, TuningStrength.Large);

            return smallOffsets.Concat(mediumOffsets).Concat(largeOffsets).Distinct().ToList();
        }

        private TunedParameter<decimal>[] ApplyDecimalOffset(decimal value, decimal offset, string name, TuningStrength strength)
        {
            var positiveOffset = new TunedParameter<decimal>(value, Math.Min(value + offset, 1), name, TuningDirection.Positive, strength);
            var negativeOffset = new TunedParameter<decimal>(value, Math.Max(value - offset, 0), name, TuningDirection.Negative, strength);

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

            var appliedSmallOffset = Math.Max((int)Math.Max(input.Value * smallOffset, min.GetValueOrDefault()), 1);
            var appliedMediumOffset = Math.Max((int)(input.Value * mediumOffset), 2);
            var appliedLargeOffset = Math.Max((int)(input.Value * largeOffset), 3);
            
            var smallOffsets = ApplyIntegerOffset(input.Value, appliedSmallOffset, name, min, TuningStrength.Small);
            var mediumOffsets = ApplyIntegerOffset(input.Value, appliedMediumOffset, name, min, TuningStrength.Medium);
            var largeOffsets = ApplyIntegerOffset(input.Value, appliedLargeOffset, name, min, TuningStrength.Large);

            return smallOffsets.Concat(mediumOffsets).Concat(largeOffsets).Distinct().ToList();
        }

        private TunedParameter<int>[] ApplyIntegerOffset(int value, int offset, string name, int? min, TuningStrength strength)
        {
            var positiveOffset = new TunedParameter<int>(value, value + offset, name, TuningDirection.Positive, strength);
            var negativeOffset = new TunedParameter<int>(value, Math.Max(value - offset, min.GetValueOrDefault(0)), name, TuningDirection.Negative, strength);

            return new[] { positiveOffset, negativeOffset };
        }

        private IReadOnlyCollection<TunedParameter<bool>> PermutateBool(bool? input, string name)
        {
            if (input == null)
            {
                return new TunedParameter<bool>[0];
            }

            var boolVal = input.GetValueOrDefault();

            return new List<TunedParameter<bool>>
            {
                new TunedParameter<bool>(boolVal, !boolVal, name, !boolVal ? TuningDirection.Positive : TuningDirection.Negative, TuningStrength.Small)
            };
        }

        private IReadOnlyCollection<TunedParameter<TimeSpan>> PermutateTimeSpans(TimeSpan? input, string name)
        {
            if (input == null)
            {
                return new TunedParameter<TimeSpan>[0];
            }
            
            var timeSpan = input.GetValueOrDefault();

            var smallOffset = 0.1m;
            var mediumOffset = 0.5m;
            var largeOffset = 1.5m;

            var appliedSmallOffset = TimeSpan.FromTicks((long)(timeSpan.Ticks * smallOffset));
            var appliedMediumOffset = TimeSpan.FromTicks((long)(timeSpan.Ticks * mediumOffset));
            var appliedLargeOffset = TimeSpan.FromTicks((long)(timeSpan.Ticks * largeOffset));

            var smallOffsets = ApplyTimeSpanOffset(timeSpan, appliedSmallOffset, name, TuningStrength.Small);
            var mediumOffsets = ApplyTimeSpanOffset(timeSpan, appliedMediumOffset, name, TuningStrength.Medium);
            var largeOffsets = ApplyTimeSpanOffset(timeSpan, appliedLargeOffset, name, TuningStrength.Large);

            return (new[]
            {
                new TunedParameter<TimeSpan>(timeSpan, TimeSpan.FromHours(1), name, CheckDirection(timeSpan, TimeSpan.FromHours(1)), TuningStrength.Custom),
                new TunedParameter<TimeSpan>(timeSpan, TimeSpan.FromDays(1), name, CheckDirection(timeSpan, TimeSpan.FromDays(1)),TuningStrength.Custom),
                new TunedParameter<TimeSpan>(timeSpan, TimeSpan.FromDays(7), name, CheckDirection(timeSpan, TimeSpan.FromDays(7)),TuningStrength.Custom),
                new TunedParameter<TimeSpan>(timeSpan, TimeSpan.FromDays(14), name, CheckDirection(timeSpan, TimeSpan.FromDays(14)),TuningStrength.Custom),
                new TunedParameter<TimeSpan>(timeSpan, TimeSpan.FromDays(28), name, CheckDirection(timeSpan, TimeSpan.FromDays(28)),TuningStrength.Custom)
            })
                .Concat(smallOffsets)
                .Concat(mediumOffsets)
                .Concat(largeOffsets)
                .Where(_ => _.TunedValue < TimeSpan.FromDays(180)) // no back test tuning over 6 months
                .Where(_ => _.TunedValue > TimeSpan.Zero)
                .Distinct()
                .ToList();
        }

        private TuningDirection CheckDirection(TimeSpan span, TimeSpan tuneSpan)
        {
            if (span == tuneSpan)
            {
                return TuningDirection.None;
            }

            if (span > tuneSpan)
            {
                return TuningDirection.Negative;
            }
            else
            {
                return TuningDirection.Positive;
            }
        }

        private TunedParameter<TimeSpan>[] ApplyTimeSpanOffset(TimeSpan value, TimeSpan offset, string name, TuningStrength strength)
        {
            var positiveOffset = new TunedParameter<TimeSpan>(value, value + offset, name, TuningDirection.Positive, strength);
            if (value < offset)
            {
                return new[] { positiveOffset };
            }

            var negativeOffset = new TunedParameter<TimeSpan>(value, value - offset, name, TuningDirection.Negative, strength);

            return new[] { positiveOffset, negativeOffset };
        }
    }
}
