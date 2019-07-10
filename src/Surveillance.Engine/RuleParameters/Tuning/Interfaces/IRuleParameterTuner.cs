using System.Collections.Generic;

namespace Surveillance.Engine.Rules.RuleParameters.Tuning.Interfaces
{
    public interface IRuleParameterTuner
    {
        IReadOnlyCollection<T> ParametersFramework<T>(T parameters) where T : class;
    }
}