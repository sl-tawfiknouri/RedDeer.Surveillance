using System.Collections.Generic;
using System.Linq;
using Domain.Financial;
using Domain.Trading;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Trades
{
    /// <summary>
    /// Buy or Sell position on an security
    /// Can mix both
    /// </summary>
    public class TradePosition : ITradePosition
    {
        private readonly IList<Order> _trades;

        public TradePosition(IList<Order> trades)
        {
            _trades = trades?.Where(trad => trad != null).ToList() ?? new List<Order>();
        }

        public IList<Order> Get()
        {
            return new List<Order>(_trades);
        }

        public void Add(Order item)
        {
            _trades.Add(item);
        }

        public long TotalVolume()
        {
            return _trades.Sum(trad => trad?.OrderFilledVolume ?? 0);
        }

        public long TotalVolumeOrderedOrFilled()
        {
            return _trades
                .Where(trad => trad != null)
                .Sum(trad => 
                    trad.OrderFilledVolume == 0 
                        ? trad.OrderOrderedVolume.GetValueOrDefault(0)
                        : (trad.OrderFilledVolume.GetValueOrDefault(0)));
        }

        public long VolumeInStatus(OrderStatus status)
        {
            return
                _trades
                .Where(trad => trad != null && trad.OrderStatus() == status)
                .Sum(trad => trad.OrderFilledVolume.GetValueOrDefault(0));
        }

        public long VolumeNotInStatus(OrderStatus status)
        {
            return
                _trades
                .Where(trad => trad != null && trad.OrderStatus() != status)
                .Sum(trad => trad.OrderFilledVolume.GetValueOrDefault(0));
        }

        /// <summary>
        /// Check if the current position (this) is a subset of the provided (arg) position
        /// uses BY REFERENCE for comparision
        /// </summary>
        public bool PositionIsSubsetOf(ITradePosition position)
        {
            if (position == null)
            {
                return false;
            }

            if (position.Get().SequenceEqual(_trades))
            {
                return true;
            }

            return !_trades.Except(position.Get()).Any();
        }
    }
}
