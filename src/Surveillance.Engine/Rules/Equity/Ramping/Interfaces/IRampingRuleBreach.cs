using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces
{
    public interface IRampingRuleBreach : IRuleBreach
    {
        IRampingStrategySummaryPanel SummaryPanel { get; set; }
    }
}
