using System;
using Surveillance.Engine.Rules.RuleParameters.Tuning.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Tuning
{
    public class TunedParameter<T> : ITunedParameter<T> where T : IEquatable<T>
    {
        public TunedParameter(T baseValue, T tunedValue, string parameterName)
        {
            BaseValue = baseValue;
            TunedValue = tunedValue;
            ParameterName = parameterName;
        }

        public T BaseValue { get; set; }
        public T TunedValue { get; set; }

        public string ParameterName { get; set; }

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
    }
}
