using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Domain.Equity.TimeBars;
using Domain.Files;
using Domain.Files.Interfaces;
using Domain.Trading;
using Microsoft.Extensions.Logging;
using TestHarness.Engine.OrderGenerator.Strategies;

namespace TestHarness.Engine.OrderGenerator
{
    public class TradingFileDataImportProcess : BaseTradingProcess
    {
        private readonly ITradeFileCsvToOrderMapper _csvToDtoMapper;
        private readonly string _filePath;

        public TradingFileDataImportProcess(
            ILogger logger,
            ITradeFileCsvToOrderMapper csvToDtoMapper,
            string filePath)
            : base(logger, new StubTradeStrategy())
        {
            _csvToDtoMapper = csvToDtoMapper ?? throw new ArgumentNullException(nameof(csvToDtoMapper));
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
                var csvRecords = csv.GetRecords<TradeFileCsv>().ToList();

                foreach (var record in csvRecords)
                {
                    var mappedRecord = _csvToDtoMapper.Map(record);
                    if (mappedRecord != null)
                    {
                        tradeOrders.Add(mappedRecord);
                    }
                }
            }

            if (_csvToDtoMapper.FailedParseTotal > 0)
            {
                Logger.LogError($"TradingFileDataImportProcess had {_csvToDtoMapper.FailedParseTotal} errors parsing the input CSV file {_filePath}");
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