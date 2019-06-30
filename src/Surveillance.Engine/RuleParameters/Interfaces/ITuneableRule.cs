using Surveillance.Engine.Rules.RuleParameters.Tuning;

namespace Surveillance.Engine.Rules.RuleParameters.Interfaces
{
    public interface ITuneableRule
    {
        TunedParameter<string> TunedParam { get; set; }
    }
}
