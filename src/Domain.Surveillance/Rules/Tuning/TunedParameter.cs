using System;
using Domain.Surveillance.Rules.Tuning.Interfaces;

namespace Domain.Surveillance.Rules.Tuning
{
    [Serializable]
    public class TunedParameter<T> : ITunedParameter<T> where T : IEquatable<T>
    {
        public TunedParameter(
            T baseValue, 
            T tunedValue,
            string parameterName,
            string baseId,
            TuningDirection direction, 
            TuningStrength strength)
        {
            BaseValue = baseValue;
            TunedValue = tunedValue;
            ParameterName = parameterName;
            BaseId = baseId ?? string.Empty;
            TuningDirection = direction;
            TuningStrength = strength;
        }

        public T BaseValue { get; set; }
        public T TunedValue { get; set; }

        public string ParameterName { get; set; }
        public string BaseId { get; set; }
        public TuningDirection TuningDirection { get; set; }
        public TuningStrength TuningStrength { get; set; }
        
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var castedObj = obj as TunedParameter<T>;

            if (castedObj == null)
            {
                return false;
            }

            return
                string.Equals(ParameterName, castedObj.ParameterName, StringComparison.OrdinalIgnoreCase)
                && BaseValue.Equals(castedObj.BaseValue)
                && TunedValue.Equals(castedObj.TunedValue);
        }

        public TunedParameter<string> MapToString()
        {
            return new TunedParameter<string>(
                BaseValue?.ToString() ?? string.Empty,
                TunedValue?.ToString() ?? string.Empty,
                ParameterName ?? string.Empty,
                BaseId,
                TuningDirection,
                TuningStrength);
        }
    }
}
