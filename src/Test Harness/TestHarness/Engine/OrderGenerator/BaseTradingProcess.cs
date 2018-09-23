using NLog;
using System;
using Domain.Equity.Frames;
using Domain.Equity.Streams.Interfaces;
using Domain.Trades.Orders;
using Domain.Trades.Streams.Interfaces;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;
// ReSharper disable InconsistentlySynchronizedField

namespace TestHarness.Engine.OrderGenerator
{
    /// <summary>
    /// Equity update driven trading process
    /// </summary>
    public abstract class BaseTradingProcess : IOrderDataGenerator
    {
        private IDisposable _unsubscriber;
        protected IStockExchangeStream StockStream;
        protected ITradeOrderStream<TradeOrderFrame> TradeStream;
        protected ITradeStrategy<TradeOrderFrame> OrderStrategy;

        private readonly object _stateTransition = new object();
        private volatile bool _generatorExecuting;

        protected readonly ILogger Logger;

        protected BaseTradingProcess(ILogger logger, ITradeStrategy<TradeOrderFrame> orderStrategy)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            OrderStrategy = orderStrategy ?? throw new ArgumentNullException(nameof(orderStrategy));
        }

        public void InitiateTrading(IStockExchangeStream stockStream, ITradeOrderStream<TradeOrderFrame> tradeStream)
        {
            lock (_stateTransition)
            {
                if (stockStream == null)
                {
                    Logger.Log(LogLevel.Error, "Initiation attempt in order data generator with null stock stream");
                    return;
                }

                if (tradeStream == null)
                {
                    Logger.Log(LogLevel.Error, "Initiation attempt in order data generator with null trade stream");
                    return;
                }

                if (_generatorExecuting)
                {
                    Logger.Log(LogLevel.Warn, "Initiating new trading with predecessor active");
                    _TerminateTrading();
                }

                Logger.Log(LogLevel.Info, "Order data generator initiated with new stock stream");
                StockStream = stockStream;
                TradeStream = tradeStream;
                _unsubscriber = stockStream.Subscribe(this);
                _generatorExecuting = true;

                _InitiateTrading();
            }
        }

        protected abstract void _InitiateTrading();

        public void OnCompleted()
        {
            Logger.Log(LogLevel.Info, "Order data generator received completed message from stock stream. Terminating order data generation");
            TerminateTrading();
        }

        public void OnError(Exception error)
        {
            Logger.Log(LogLevel.Error, error);
        }

        public abstract void OnNext(ExchangeFrame value);

        /// <summary>
        /// Avoid calling this from inside another state transition
        /// </summary>
        public void TerminateTrading()
        {
            lock (_stateTransition)
            {
                _TerminateTrading();
            }
        }

        /// <summary>
        /// Avoid dead locks with initiation terminating old trades
        /// </summary>
        private void _TerminateTrading()
        {
            Logger.Log(LogLevel.Info, "Order data generator terminating trading");

            _unsubscriber?.Dispose();

            StockStream = null;
            TradeStream = null;
            _generatorExecuting = false;

            _TerminateTradingStrategy();
        }

        protected abstract void _TerminateTradingStrategy();
    }
}
