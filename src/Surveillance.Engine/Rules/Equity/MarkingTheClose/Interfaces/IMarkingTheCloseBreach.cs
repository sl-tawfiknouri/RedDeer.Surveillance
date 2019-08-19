namespace Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces
{
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Universe.MarketEvents;

    public interface IMarkingTheCloseBreach : IRuleBreach
    {
        VolumeBreach DailyBreach { get; }

        IMarkingTheCloseEquitiesParameters EquitiesParameters { get; }

        MarketOpenClose MarketClose { get; }

        VolumeBreach WindowBreach { get; }
    }
}