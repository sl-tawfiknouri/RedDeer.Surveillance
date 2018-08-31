using Domain.Equity.Trading.Orders;
using Surveillance.Trades.Interfaces;
using System;
using System.Collections.Generic;

namespace Surveillance.Trades
{
    public class TradingHistoryStack : ITradingHistoryStack
    {
        private Stack<TradeOrderFrame> _activeStack;
        private Queue<TradeOrderFrame> _history;

        private object _lock = new object();
        private TimeSpan _activeTradeDuration;

        public TradingHistoryStack(TimeSpan activeTradeDuration)
        {
            _activeStack = new Stack<TradeOrderFrame>();
            _history = new Queue<TradeOrderFrame>();
            _activeTradeDuration = activeTradeDuration;
        }

        public void Add(TradeOrderFrame frame, DateTime currentTime)
        {
            if (frame == null)
            {
                return;
            }

            lock (_lock)
            {
                if (currentTime.Subtract(frame.StatusChangedOn) <= _activeTradeDuration)
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
                var counterpartyStack = new Stack<TradeOrderFrame>();

                while (initialActiveStackCount > 0)
                {
                    var poppedItem = _activeStack.Pop();
                    if (currentTime.Subtract(poppedItem.StatusChangedOn) > _activeTradeDuration)
                    {
                        _history.Enqueue(poppedItem);
                    }
                    else
                    {
                        counterpartyStack.Push(poppedItem);
                    }

                    initialActiveStackCount--;
                }

                var counterpartyStackCount = counterpartyStack.Count;

                while (counterpartyStackCount > 0)
                {
                    var replayItem = counterpartyStack.Pop();
                    _activeStack.Push(replayItem);

                    counterpartyStackCount--;
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
                var copyStack = new Stack<TradeOrderFrame>(_activeStack);
                var copyStacker = new Stack<TradeOrderFrame>(copyStack);

                // copy stack reverses the order..
                return copyStacker;
            }
        }
    }
}
