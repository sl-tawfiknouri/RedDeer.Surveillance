namespace Domain.Surveillance.Rules.Tuning.Interfaces
{
    public interface ITunedParameter<T>
    {
        T BaseValue { get; set; }

        string ParameterName { get; set; }

        T TunedValue { get; set; }

        TuningDirection TuningDirection { get; set; }

        TuningStrength TuningStrength { get; set; }

        TunedParameter<string> MapToString();
    }
}