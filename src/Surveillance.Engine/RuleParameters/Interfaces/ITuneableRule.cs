using Surveillance.Engine.Rules.RuleParameters.Tuning;

namespace Surveillance.Engine.Rules.RuleParameters.Interfaces
{
    public interface ITuneableRule
    {
        bool IsTuned { get; set; }
        TunedParameter<string> TunedParam { get; set; }
    }
}
