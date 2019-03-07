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

namespace TestHarness.Engine.OrderGenerator
{
    public class TradingFileDataImportProcess : BaseTradingProcess
    {
        private readonly IOrderFileToOrderSerialiser _orderFileToOrderSerialiser;
        private readonly string _filePath;

        public TradingFileDataImportProcess(
            ILogger logger,
            IOrderFileToOrderSerialiser orderFileToOrderSerialiser,
            string filePath)
            : base(logger, new StubTradeStrategy())
        {
            _orderFileToOrderSerialiser = orderFileToOrderSerialiser ?? throw new ArgumentNullException(nameof(orderFileToOrderSerialiser));
            _filePath = filePath ?? string.Empty;
        }

        protected override void _InitiateTrading()
        {
            if (string.IsNullOrWhiteSpace(_filePath))
            {
                Logger.LogError("Trading File Data Import Process did not find file because the path was empty or null");
                return;
            }

            if (!File.Exists(_filePath))
            {
                Logger.LogError($"Trading File Data Import Process did not find file {_filePath}");
                return;
            }

            var tradeOrders = new List<Order>();

            using (var reader = File.OpenText(_filePath))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.HasHeaderRecord = true;
                var csvRecords = csv.GetRecords<OrderFileContract>().ToList();

                foreach (var record in csvRecords)
                {
                    var mappedRecord = _orderFileToOrderSerialiser.Map(record);
                    if (mappedRecord != null)
                    {
                        tradeOrders.Add(mappedRecord);
                    }
                }
            }

            if (_orderFileToOrderSerialiser.FailedParseTotal > 0)
            {
                Logger.LogError($"TradingFileDataImportProcess had {_orderFileToOrderSerialiser.FailedParseTotal} errors parsing the input CSV file {_filePath}");
            }

            if (!tradeOrders.Any())
            {
                return;
            }

            var sortedTradeOrders = tradeOrders.OrderBy(to => to.PlacedDate).ToList();

            foreach (var item in sortedTradeOrders)
            {
                TradeStream.Add(item);
            }
        }

        public override void OnNext(EquityIntraDayTimeBarCollection value)
        {
        }

        protected override void _TerminateTradingStrategy()
        {
        }
    }
}