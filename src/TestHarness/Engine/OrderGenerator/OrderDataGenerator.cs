using Domain.Equity.Trading;
using NLog;
using System;

namespace TestHarness.Engine.OrderGenerator
{
    public class OrderDataGenerator : IObserver<ExchangeTick>
    {
        private IDisposable _unsubscriber;
        private IStockExchangeStream _stockStream;

        private object _stateTransition = new object();
        private volatile bool _generatorExecuting;

        private ILogger _logger;

        public OrderDataGenerator(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void InitiateTrading(IStockExchangeStream stockStream)
        {
            lock (_stateTransition)
            {
                if (stockStream == null)
                {
                    _logger.Log(LogLevel.Error, "Initiation attempt in order data generator with null stock stream");
                    return;
                }

                if (_generatorExecuting)
                {
                    _logger.Log(LogLevel.Warn, "Initiating new trading with predecessor active");
                    _TerminateTrading();
                }

                _logger.Log(LogLevel.Info, "Order data generator initiated with new stock stream");
                _stockStream = stockStream;
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

        public void OnNext(ExchangeTick value)
        {
            throw new NotImplementedException();
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
            _generatorExecuting = false;
        }
    }
}
