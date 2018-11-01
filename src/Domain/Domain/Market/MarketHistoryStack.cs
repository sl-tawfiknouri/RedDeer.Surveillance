using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Equity.Frames;
using Domain.Market.Interfaces;

namespace Domain.Market
{
    public class MarketHistoryStack : IMarketHistoryStack
    {
        private readonly Stack<ExchangeFrame> _activeStack;
        private readonly Queue<ExchangeFrame> _history;
        private StockExchange _market;

        private readonly object _lock = new object();
        private readonly TimeSpan _activeTradeDuration;

        public MarketHistoryStack(TimeSpan activeTradeDuration)
        {
            _activeStack = new Stack<ExchangeFrame>();
            _history = new Queue<ExchangeFrame>();
            _activeTradeDuration = activeTradeDuration;
        }

        public void Add(ExchangeFrame frame, DateTime currentTime)
        {
            if (frame == null)
            {
                return;
            }

            lock (_lock)
            {
                if (currentTime.Subtract(frame.TimeStamp) <= _activeTradeDuration)
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
                var counterPartyStack = new Stack<ExchangeFrame>();

                while (initialActiveStackCount > 0)
                {
                    var poppedItem = _activeStack.Pop();
                    if (currentTime.Subtract(poppedItem.TimeStamp) > _activeTradeDuration)
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
        public Stack<ExchangeFrame> ActiveMarketHistory()
        {
            lock (_lock)
            {
                var tradeStackCopy = new Stack<ExchangeFrame>(_activeStack);
                var reverseCopyOfTradeStack = new Stack<ExchangeFrame>(tradeStackCopy);

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
            _market = _activeStack?.Peek()?.Exchange
                ?? _history?.FirstOrDefault()?.Exchange;

            return _market;
        }
    }
}
