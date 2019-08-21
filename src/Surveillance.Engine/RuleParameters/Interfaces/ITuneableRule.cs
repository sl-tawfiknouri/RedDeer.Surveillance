namespace Surveillance.Engine.Rules.RuleParameters.Interfaces
{
    using Domain.Surveillance.Rules.Tuning;

    public interface ITuneableRule
    {
        bool PerformTuning { get; set; }

        TunedParameter<string> TunedParam { get; set; }
    }
}