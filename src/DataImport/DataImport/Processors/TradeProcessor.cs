using System;
using DataImport.Processors.Interfaces;
using Domain.Trades.Orders;
using Domain.Trades.Streams.Interfaces;
using Microsoft.Extensions.Logging;

namespace DataImport.Processors
{
    /// <summary>
    /// Internal logic for the relay acting on the trades stream
    /// Decorates its own trade order stream internally
    /// </summary>
    public class TradeProcessor<T> : ITradeProcessor<T>
    {
        private readonly ILogger _logger;
        private readonly ITradeOrderStream<T> _tradeOrderStream;

        public TradeProcessor(
            ILogger<TradeProcessor<TradeOrderFrame>> logger,
            ITradeOrderStream<T> tradeOrderStream)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tradeOrderStream = tradeOrderStream ?? throw new ArgumentNullException(nameof(tradeOrderStream));
        }

        public void OnCompleted()
        {
            _logger.LogInformation("Relay Trade Processor source stream reached completion.");
        }

        public void OnError(Exception error)
        {
            _logger.LogError("Relay Trade Processor read in an exception from its source stream.", error);
        }

        public void OnNext(T value)
        {
            if (value == null)
            {
                _logger.LogInformation("Relay Trade Processor was passed a null trade frame value.");
            }

            // do your stuff, now broadcast it onwards

            _tradeOrderStream.Add(value);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _tradeOrderStream.Subscribe(observer);
        }
    }
}
