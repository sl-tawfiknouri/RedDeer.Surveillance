using Surveillance.Trades.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Domain.Trades.Orders;

namespace Surveillance.Trades
{
    /// <summary>
    /// Buy or Sell position on an security
    /// </summary>
    public class TradePosition : ITradePosition
    {
        private readonly IList<TradeOrderFrame> _trades;

        public TradePosition(IList<TradeOrderFrame> trades)
        {
            _trades = trades?.Where(trad => trad != null).ToList() ?? new List<TradeOrderFrame>();
        }

        public IList<TradeOrderFrame> Get()
        {
            return new List<TradeOrderFrame>(_trades);
        }

        public void Add(TradeOrderFrame item)
        {
            _trades.Add(item);
        }

        public int TotalVolume()
        {
            return _trades.Sum(trad => trad?.Volume ?? 0);
        }

        public int VolumeInStatus(OrderStatus status)
        {
            return
                _trades
                .Where(trad => trad != null && trad.OrderStatus == status)
                .Sum(trad => trad.Volume);
        }

        public int VolumeNotInStatus(OrderStatus status)
        {
            return
                _trades
                .Where(trad => trad != null && trad.OrderStatus != status)
                .Sum(trad => trad.Volume);
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

            return !_trades.Except(position.Get()).Any();
        }
    }
}
