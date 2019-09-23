// ReSharper disable InconsistentlySynchronizedField

namespace TestHarness.Engine.OrderGenerator
{
    using System;

    using Domain.Core.Markets.Collections;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Streams.Interfaces;

    using Microsoft.Extensions.Logging;

    using TestHarness.Engine.OrderGenerator.Interfaces;
    using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

    /// <summary>
    ///     Equity update driven trading process
    /// </summary>
    public abstract class BaseTradingProcess : IOrderDataGenerator
    {
        protected readonly ILogger Logger;

        protected ITradeStrategy<Order> OrderStrategy;

        protected IStockExchangeStream StockStream;

        protected IOrderStream<Order> TradeStream;

        private readonly object _stateTransition = new object();

        private volatile bool _generatorExecuting;

        private IDisposable _unsubscriber;

        protected BaseTradingProcess(ILogger logger, ITradeStrategy<Order> orderStrategy)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.OrderStrategy = orderStrategy ?? throw new ArgumentNullException(nameof(orderStrategy));
        }

        public void InitiateTrading(IStockExchangeStream stockStream, IOrderStream<Order> tradeStream)
        {
            lock (this._stateTransition)
            {
                if (stockStream == null)
                {
                    this.Logger.Log(
                        LogLevel.Error,
                        "Initiation attempt in order data generator with null stock stream");
                    return;
                }

                if (tradeStream == null)
                {
                    this.Logger.Log(
                        LogLevel.Error,
                        "Initiation attempt in order data generator with null trade stream");
                    return;
                }

                if (this._generatorExecuting)
                {
                    this.Logger.LogInformation("Initiating new trading with predecessor active");
                    this._TerminateTrading();
                }

                this.Logger.LogInformation("Order data generator initiated with new stock stream");
                this.StockStream = stockStream;
                this.TradeStream = tradeStream;
                this._unsubscriber = stockStream.Subscribe(this);
                this._generatorExecuting = true;

                this._InitiateTrading();
            }
        }

        public void OnCompleted()
        {
            this.Logger.LogInformation(
                "Order data generator received completed message from stock stream. Terminating order data generation");
            this.TerminateTrading();
        }

        public void OnError(Exception error)
        {
            this.Logger.LogError(error?.Message);
        }

        public abstract void OnNext(EquityIntraDayTimeBarCollection value);

        /// <summary>
        ///     Avoid calling this from inside another state transition
        /// </summary>
        public void TerminateTrading()
        {
            lock (this._stateTransition)
            {
                this._TerminateTrading();
            }
        }

        protected abstract void _InitiateTrading();

        protected abstract void _TerminateTradingStrategy();

        /// <summary>
        ///     Avoid dead locks with initiation terminating old trades
        /// </summary>
        private void _TerminateTrading()
        {
            this.Logger.LogInformation("Order data generator terminating trading");

            this._unsubscriber?.Dispose();

            this.StockStream = null;
            this.TradeStream = null;
            this._generatorExecuting = false;

            this._TerminateTradingStrategy();
        }
    }
}