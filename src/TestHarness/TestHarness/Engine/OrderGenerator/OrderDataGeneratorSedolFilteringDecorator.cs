namespace TestHarness.Engine.OrderGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Markets.Collections;
    using Domain.Core.Markets.Timebars;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Streams.Interfaces;

    using TestHarness.Engine.OrderGenerator.Interfaces;
    using TestHarness.Factory.EquitiesFactory.Interfaces;

    public class OrderDataGeneratorSedolFilteringDecorator : IOrderDataGenerator
    {
        private readonly IOrderDataGenerator _baseGenerator;

        private readonly bool _inclusive;

        private readonly IReadOnlyCollection<string> _sedols;

        private readonly IStockExchangeStreamFactory _streamFactory;

        private IStockExchangeStream _stream;

        private IDisposable _unsubscriber;

        public OrderDataGeneratorSedolFilteringDecorator(
            IStockExchangeStreamFactory streamFactory,
            IOrderDataGenerator baseGenerator,
            IReadOnlyCollection<string> sedols,
            bool inclusive)
        {
            this._streamFactory = streamFactory ?? throw new ArgumentNullException(nameof(streamFactory));
            this._baseGenerator = baseGenerator ?? throw new ArgumentNullException(nameof(baseGenerator));
            this._sedols = sedols?.Where(sed => !string.IsNullOrWhiteSpace(sed)).ToList() ?? new List<string>();
            this._inclusive = inclusive;
        }

        public void InitiateTrading(IStockExchangeStream stockStream, IOrderStream<Order> tradeStream)
        {
            this._unsubscriber = stockStream.Subscribe(this);
            this._stream = this._streamFactory.Create();
            this._baseGenerator.InitiateTrading(this._stream, tradeStream);
        }

        public void OnCompleted()
        {
            this._baseGenerator.OnCompleted();
        }

        public void OnError(Exception error)
        {
            this._baseGenerator.OnError(error);
        }

        public void OnNext(EquityIntraDayTimeBarCollection value)
        {
            if (value == null)
            {
                this._baseGenerator.OnNext(null);
                return;
            }

            var filteredSecurities =
                value.Securities?.Where(
                    sec => this._sedols.Contains(
                               sec?.Security.Identifiers.Sedol,
                               StringComparer.CurrentCultureIgnoreCase) == this._inclusive).ToList()
                ?? new List<EquityInstrumentIntraDayTimeBar>();

            var filteredFrame = new EquityIntraDayTimeBarCollection(value.Exchange, value.Epoch, filteredSecurities);

            this._baseGenerator.OnNext(filteredFrame);
        }

        public void TerminateTrading()
        {
            this._baseGenerator.TerminateTrading();
        }
    }
}