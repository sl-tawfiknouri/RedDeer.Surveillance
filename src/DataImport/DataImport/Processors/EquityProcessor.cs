using System;
using DataImport.Processors.Interfaces;
using Domain.Equity.Frames;
using Domain.Equity.Streams.Interfaces;
using Microsoft.Extensions.Logging;

namespace DataImport.Processors
{
    public class EquityProcessor : IEquityProcessor<ExchangeFrame>
    {
        private readonly ILogger _logger;
        private readonly IStockExchangeStream _exchangeStream;

        public EquityProcessor(
            ILogger<EquityProcessor> logger,
            IStockExchangeStream exchangeStream)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _exchangeStream = exchangeStream ?? throw new ArgumentNullException(nameof(exchangeStream));
        }

        public void OnCompleted()
        {
            _logger.LogInformation("Relay Equity Processor source stream reached completion.");
        }

        public void OnError(Exception error)
        {
            _logger.LogError("Relay Equity Processor read in an exception from its source stream.", error);
        }

        public void OnNext(ExchangeFrame value)
        {
            if (value == null)
            {
                _logger.LogInformation("Relay Equity Processor was passed a null exchange frame value.");
            }

            // do your stuff, now broadcast it onwards

            _exchangeStream.Add(value);
        }

        public IDisposable Subscribe(IObserver<ExchangeFrame> observer)
        {
            return _exchangeStream.Subscribe(observer);
        }
    }
}
