namespace Surveillance.Engine.Rules.RuleParameters.Tuning.Interfaces
{
    public interface ITunedParameter<T>
    {
        T BaseValue { get; set; }
        T TunedValue { get; set; }

        string ParameterName { get; set; }

        TuningDirection TuningDirection { get; set; }
        TuningStrength TuningStrength { get; set; }
        TunedParameter<string> MapToString();
    }
}
