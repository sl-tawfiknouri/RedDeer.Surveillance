using Surveillance.Engine.Rules.RuleParameters.Tuning.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Tuning
{
    public class TunedParameter<T> : ITunedParameter<T>
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
    }
}
