namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces
{
    using Surveillance.Engine.Rules.Rules.Interfaces;

    public interface IRampingRuleBreach : IRuleBreach
    {
        IRampingStrategySummaryPanel SummaryPanel { get; set; }
    }
}