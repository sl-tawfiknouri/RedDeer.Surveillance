using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Markets.Collections;
using Domain.Core.Markets.Timebars;
using Domain.Core.Trading.Orders;
using Domain.Surveillance.Streams.Interfaces;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Factory.EquitiesFactory;
using TestHarness.Factory.EquitiesFactory.Interfaces;

namespace TestHarness.Engine.OrderGenerator
{
    public class OrderDataGeneratorSedolFilteringDecorator : IOrderDataGenerator
    {
        private readonly IOrderDataGenerator _baseGenerator;
        private readonly IReadOnlyCollection<string> _sedols;
        private readonly IStockExchangeStreamFactory _streamFactory;
        private readonly bool _inclusive;

        private IDisposable _unsubscriber;

        private IStockExchangeStream _stream;

        public OrderDataGeneratorSedolFilteringDecorator(
            IStockExchangeStreamFactory streamFactory,
            IOrderDataGenerator baseGenerator,
            IReadOnlyCollection<string> sedols,
            bool inclusive)
        {
            _streamFactory = streamFactory ?? throw new ArgumentNullException(nameof(streamFactory));
            _baseGenerator = baseGenerator ?? throw new ArgumentNullException(nameof(baseGenerator));
            _sedols = sedols?.Where(sed => !string.IsNullOrWhiteSpace(sed)).ToList() ?? new List<string>();
            _inclusive = inclusive;
        }

        public void OnCompleted()
        {
            _baseGenerator.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _baseGenerator.OnError(error);
        }

        public void OnNext(EquityIntraDayTimeBarCollection value)
        {
            if (value == null)
            {
                _baseGenerator.OnNext(null);
                return;
            }

            var filteredSecurities =
                value
                    .Securities
                    ?.Where(sec =>
                        _sedols.Contains(sec?.Security.Identifiers.Sedol, StringComparer.CurrentCultureIgnoreCase) == _inclusive)
                    .ToList()
                ?? new List<EquityInstrumentIntraDayTimeBar>();

            var filteredFrame = new EquityIntraDayTimeBarCollection(value.Exchange, value.Epoch, filteredSecurities);

            _baseGenerator.OnNext(filteredFrame);
        }

        public void InitiateTrading(IStockExchangeStream stockStream, IOrderStream<Order> tradeStream)
        {
            _unsubscriber = stockStream.Subscribe(this);
            _stream = _streamFactory.Create();
            _baseGenerator.InitiateTrading(_stream, tradeStream);
        }

        public void TerminateTrading()
        {
            _baseGenerator.TerminateTrading();
        }
    }
}
