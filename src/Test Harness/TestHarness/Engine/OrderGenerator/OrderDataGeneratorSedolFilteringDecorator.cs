using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Equity.Streams.Interfaces;
using DomainV2.Equity.TimeBars;
using DomainV2.Streams.Interfaces;
using DomainV2.Trading;
using TestHarness.Engine.OrderGenerator.Interfaces;

namespace TestHarness.Engine.OrderGenerator
{
    public class OrderDataGeneratorSedolFilteringDecorator : IOrderDataGenerator
    {
        private readonly IOrderDataGenerator _baseGenerator;
        private readonly IReadOnlyCollection<string> _sedols;

        public OrderDataGeneratorSedolFilteringDecorator(
            IOrderDataGenerator baseGenerator,
            IReadOnlyCollection<string> sedols)
        {
            _baseGenerator = baseGenerator ?? throw new ArgumentNullException(nameof(baseGenerator));
            _sedols = sedols?.Where(sed => !string.IsNullOrWhiteSpace(sed)).ToList() ?? new List<string>();
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
                        !_sedols.Contains(sec?.Security.Identifiers.Sedol, StringComparer.CurrentCultureIgnoreCase))
                    .ToList()
                ?? new List<EquityInstrumentIntraDayTimeBar>();

            var filteredFrame = new EquityIntraDayTimeBarCollection(value.Exchange, value.Epoch, filteredSecurities);

            _baseGenerator.OnNext(filteredFrame);
        }

        public void InitiateTrading(IStockExchangeStream stockStream, IOrderStream<Order> tradeStream)
        {
            _baseGenerator.InitiateTrading(stockStream, tradeStream);
        }

        public void TerminateTrading()
        {
            _baseGenerator.TerminateTrading();
        }
    }
}
