using System;
using Domain.Trades.Orders;

namespace Surveillance.Universe.Interfaces
{
    public interface IUniversePlayer : IObservable<TradeOrderFrame>
    {
        void Play(IUniverse universe);
    }
}
