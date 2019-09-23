namespace Surveillance.Engine.Rules.RuleParameters.Tuning.Interfaces
{
    using System.Collections.Generic;

    public interface IRuleParameterTuner
    {
        IReadOnlyCollection<T> ParametersFramework<T>(T parameters)
            where T : class;
    }
}