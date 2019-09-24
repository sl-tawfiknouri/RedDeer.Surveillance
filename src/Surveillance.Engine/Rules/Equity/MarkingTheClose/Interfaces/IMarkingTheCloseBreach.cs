namespace Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces
{
    using Surveillance.Data.Universe.MarketEvents;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    public interface IMarkingTheCloseBreach : IRuleBreach
    {
        VolumeBreach DailyBreach { get; }

        IMarkingTheCloseEquitiesParameters EquitiesParameters { get; }

        MarketOpenClose MarketClose { get; }

        VolumeBreach WindowBreach { get; }
    }
}