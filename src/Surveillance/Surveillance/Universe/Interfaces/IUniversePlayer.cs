using System;
using Domain.Equity.Frames;
using Domain.Trades.Orders;

namespace Surveillance.Universe.Interfaces
{
    public interface IUniversePlayer : IObservable<TradeOrderFrame>, IObservable<ExchangeFrame>
    {
        void Play(IUniverse universe);
    }
}
