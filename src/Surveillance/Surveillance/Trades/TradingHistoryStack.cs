using Surveillance.Trades.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Market;
using Domain.Trades.Orders;

namespace Surveillance.Trades
{
    public class TradingHistoryStack : ITradingHistoryStack
    {
        private readonly Stack<TradeOrderFrame> _activeStack;
        private readonly Queue<TradeOrderFrame> _history;
        private StockExchange _market;

        private readonly object _lock = new object();
        private readonly TimeSpan _activeTradeDuration;
        private readonly Func<TradeOrderFrame, DateTime> _getFrameTime;
        
        public TradingHistoryStack(
            TimeSpan activeTradeDuration,
            Func<TradeOrderFrame, DateTime> getFrameTime)
        {
            _activeStack = new Stack<TradeOrderFrame>();
            _history = new Queue<TradeOrderFrame>();
            _activeTradeDuration = activeTradeDuration;
            _getFrameTime = getFrameTime;
        }

        public void Add(TradeOrderFrame frame, DateTime currentTime)
        {
            if (frame == null)
            {
                return;
            }

            lock (_lock)
            {
                if (currentTime.Subtract(_getFrameTime(frame)) <= _activeTradeDuration)
                {
                    _activeStack.Push(frame);
                }
                else
                {
                    _history.Enqueue(frame);
                }
            }
        }

        public void ArchiveExpiredActiveItems(DateTime currentTime)
        {
            lock (_lock)
            {
                var initialActiveStackCount = _activeStack.Count;
                var counterPartyStack = new Stack<TradeOrderFrame>();

                while (initialActiveStackCount > 0)
                {
                    var poppedItem = _activeStack.Pop();
                    if (currentTime.Subtract(_getFrameTime(poppedItem)) > _activeTradeDuration)
                    {
                        _history.Enqueue(poppedItem);
                    }
                    else
                    {
                        counterPartyStack.Push(poppedItem);
                    }

                    initialActiveStackCount--;
                }

                var counterPartyStackCount = counterPartyStack.Count;

                while (counterPartyStackCount > 0)
                {
                    var replayItem = counterPartyStack.Pop();
                    _activeStack.Push(replayItem);

                    counterPartyStackCount--;
                }
            }
        }

        /// <summary>
        /// Does not provide access to the underlying collection via reference
        /// Instead it returns a new list with the same underlying elements
        /// </summary>
        public Stack<TradeOrderFrame> ActiveTradeHistory()
        {
            lock (_lock)
            {
                var tradeStackCopy = new Stack<TradeOrderFrame>(_activeStack);
                var reverseCopyOfTradeStack = new Stack<TradeOrderFrame>(tradeStackCopy);

                // copy twice in order to restore initial order of elements
                return reverseCopyOfTradeStack;
            }
        }

        public StockExchange Exchange()
        {
            if (_market != null)
            {
                return _market;
            }
            
            // ReSharper disable once InconsistentlySynchronizedField
            _market = _activeStack?.Peek()?.Market
                ?? _history?.FirstOrDefault()?.Market;

            return _market;
        }
    }
}
