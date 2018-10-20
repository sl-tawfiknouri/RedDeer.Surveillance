using Surveillance.Rules.Interfaces;
using Surveillance.Universe.MarketEvents;

namespace Surveillance.Rules.MarkingTheClose.Interfaces
{
    public interface IMarkingTheCloseBreach : IRuleBreach
    {
        MarketOpenClose MarketClose { get; }
        IMarkingTheCloseParameters Parameters { get; }

        VolumeBreach DailyBreach { get; }
        VolumeBreach WindowBreach { get; }
    }
}