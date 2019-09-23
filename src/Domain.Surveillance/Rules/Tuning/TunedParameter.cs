namespace Domain.Surveillance.Rules.Tuning
{
    using System;

    using Domain.Surveillance.Rules.Tuning.Interfaces;

    [Serializable]
    public class TunedParameter<T> : ITunedParameter<T>
        where T : IEquatable<T>
    {
        public TunedParameter(
            T baseValue,
            T tunedValue,
            string parameterName,
            string baseId,
            string tuningParameterId,
            TuningDirection direction,
            TuningStrength strength)
        {
            this.BaseValue = baseValue;
            this.TunedValue = tunedValue;
            this.ParameterName = parameterName;
            this.BaseId = baseId ?? string.Empty;
            this.TuningParameterId = tuningParameterId ?? string.Empty;
            this.TuningDirection = direction;
            this.TuningStrength = strength;
        }

        public string BaseId { get; set; }

        public T BaseValue { get; set; }

        public string ParameterName { get; set; }

        public T TunedValue { get; set; }

        public TuningDirection TuningDirection { get; set; }

        public string TuningParameterId { get; set; }

        public TuningStrength TuningStrength { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var castedObj = obj as TunedParameter<T>;

            if (castedObj == null) return false;

            return string.Equals(this.ParameterName, castedObj.ParameterName, StringComparison.OrdinalIgnoreCase)
                   && this.BaseValue.Equals(castedObj.BaseValue) && this.TunedValue.Equals(castedObj.TunedValue);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public TunedParameter<string> MapToString()
        {
            return new TunedParameter<string>(
                this.BaseValue?.ToString() ?? string.Empty,
                this.TunedValue?.ToString() ?? string.Empty,
                this.ParameterName ?? string.Empty,
                this.BaseId,
                this.TuningParameterId,
                this.TuningDirection,
                this.TuningStrength);
        }
    }
}