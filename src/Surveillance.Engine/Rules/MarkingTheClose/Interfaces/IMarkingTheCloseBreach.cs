using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Universe.MarketEvents;

namespace Surveillance.Engine.Rules.Rules.MarkingTheClose.Interfaces
{
    public interface IMarkingTheCloseBreach : IRuleBreach
    {
        MarketOpenClose MarketClose { get; }
        IMarkingTheCloseParameters Parameters { get; }

        VolumeBreach DailyBreach { get; }
        VolumeBreach WindowBreach { get; }
    }
}