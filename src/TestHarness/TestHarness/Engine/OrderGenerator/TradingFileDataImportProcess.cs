namespace TestHarness.Engine.OrderGenerator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using CsvHelper;

    using Domain.Core.Markets.Collections;
    using Domain.Core.Trading.Orders;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Files.Orders;
    using SharedKernel.Files.Orders.Interfaces;

    using TestHarness.Engine.OrderGenerator.Strategies;

    public class TradingFileDataImportProcess : BaseTradingProcess
    {
        private readonly string _filePath;

        private readonly IOrderFileToOrderSerialiser _orderFileToOrderSerialiser;

        public TradingFileDataImportProcess(
            ILogger logger,
            IOrderFileToOrderSerialiser orderFileToOrderSerialiser,
            string filePath)
            : base(logger, new StubTradeStrategy())
        {
            this._orderFileToOrderSerialiser = orderFileToOrderSerialiser
                                               ?? throw new ArgumentNullException(nameof(orderFileToOrderSerialiser));
            this._filePath = filePath ?? string.Empty;
        }

        public override void OnNext(EquityIntraDayTimeBarCollection value)
        {
        }

        protected override void _InitiateTrading()
        {
            if (string.IsNullOrWhiteSpace(this._filePath))
            {
                this.Logger.LogError(
                    "Trading File Data Import Process did not find file because the path was empty or null");
                return;
            }

            if (!File.Exists(this._filePath))
            {
                this.Logger.LogError($"Trading File Data Import Process did not find file {this._filePath}");
                return;
            }

            var tradeOrders = new List<Order>();

            using (var reader = File.OpenText(this._filePath))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.HasHeaderRecord = true;
                var csvRecords = csv.GetRecords<OrderFileContract>().ToList();

                foreach (var record in csvRecords)
                {
                    var mappedRecord = this._orderFileToOrderSerialiser.Map(record);
                    if (mappedRecord != null) tradeOrders.Add(mappedRecord);
                }
            }

            if (this._orderFileToOrderSerialiser.FailedParseTotal > 0)
                this.Logger.LogError(
                    $"TradingFileDataImportProcess had {this._orderFileToOrderSerialiser.FailedParseTotal} errors parsing the input CSV file {this._filePath}");

            if (!tradeOrders.Any()) return;

            var sortedTradeOrders = tradeOrders.OrderBy(to => to.PlacedDate).ToList();

            foreach (var item in sortedTradeOrders) this.TradeStream.Add(item);
        }

        protected override void _TerminateTradingStrategy()
        {
        }
    }
}