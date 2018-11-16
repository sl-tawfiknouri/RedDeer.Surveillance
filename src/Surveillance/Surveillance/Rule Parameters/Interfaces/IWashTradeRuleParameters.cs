using System;

namespace Surveillance.Rule_Parameters.Interfaces
{
    public interface IWashTradeRuleParameters : IFilterableRule
    {
        TimeSpan WindowSize { get; }
    }
}
