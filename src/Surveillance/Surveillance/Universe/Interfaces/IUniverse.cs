using System.Collections.Generic;
using Domain.Equity.Frames;
using Domain.Trades.Orders;

namespace Surveillance.Universe.Interfaces
{
    public interface IUniverse
    {
        IReadOnlyCollection<TradeOrderFrame> Trades { get; }
        IReadOnlyCollection<IUniverseEvent> UniverseEvents { get; }
    }
}