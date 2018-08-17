using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Streams.Interfaces;
using NLog;
using System;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Engine.OrderGenerator.Strategies;

namespace TestHarness.Engine.OrderGenerator
{
    public class OrderDataGenerator : IOrderDataGenerator
    {
        private IDisposable _unsubscriber;
        private IStockExchangeStream _stockStream;
        private ITradeOrderStream _tradeStream;
        private ITradeStrategy _orderStrategy;

        private object _stateTransition = new object();
        private volatile bool _generatorExecuting;

        private ILogger _logger;

        public OrderDataGenerator(ILogger logger, ITradeStrategy orderStrategy)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _orderStrategy = orderStrategy ?? throw new ArgumentNullException(nameof(orderStrategy));
        }

        public void InitiateTrading(IStockExchangeStream stockStream, ITradeOrderStream tradeStream)
        {
            lock (_stateTransition)
            {
                if (stockStream == null)
                {
                    _logger.Log(LogLevel.Error, "Initiation attempt in order data generator with null stock stream");
                    return;
                }

                if (tradeStream == null)
                {
                    _logger.Log(LogLevel.Error, "Initiation attempt in order data generator with null trade stream");
                    return;
                }

                if (_generatorExecuting)
                {
                    _logger.Log(LogLevel.Warn, "Initiating new trading with predecessor active");
                    _TerminateTrading();
                }

                _logger.Log(LogLevel.Info, "Order data generator initiated with new stock stream");
                _stockStream = stockStream;
                _tradeStream = tradeStream;
                _unsubscriber = stockStream.Subscribe(this);
                _generatorExecuting = true;
            }
        }

        public void OnCompleted()
        {
            _logger.Log(LogLevel.Info, "Order data generator received completed message from stock stream. Terminating order data generation");
            this.TerminateTrading();
        }

        public void OnError(Exception error)
        {
            if (error == null)
            {
                _logger.Log(LogLevel.Error, "Order data generator receieved a null exception OnError");
                return;
            }

            _logger.Log(LogLevel.Error, error);
        }

        public void OnNext(ExchangeFrame value)
        {
            if (value == null)
            {
                return;
            }

            _orderStrategy.ExecuteTradeStrategy(value, _tradeStream);
        }

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
            _logger.Log(LogLevel.Info, "Order data generator terminating trading");

            if (_unsubscriber != null)
            {
                _unsubscriber.Dispose();
            }

            _stockStream = null;
            _tradeStream = null;
            _generatorExecuting = false;
        }
    }
}
