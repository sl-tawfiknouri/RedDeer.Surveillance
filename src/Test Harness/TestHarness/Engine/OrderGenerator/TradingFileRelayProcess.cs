using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Domain.Equity.Frames;
using Domain.Trades.Orders;
using Domain.Trades.Orders.Interfaces;
using NLog;
using TestHarness.Engine.OrderGenerator.Strategies;

namespace TestHarness.Engine.OrderGenerator
{
    public class TradingFileRelayProcess : BaseTradingProcess
    {
        private readonly ITradeOrderCsvToDtoMapper _csvToDtoMapper;
        private readonly string _filePath;

        public TradingFileRelayProcess(
            ILogger logger,
            ITradeOrderCsvToDtoMapper csvToDtoMapper,
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
                Logger.Error($"Trading File Relay Process did not find file because the path was empty or null");
                return;
            }

            if (!File.Exists(_filePath))
            {
                Logger.Error($"Trading File Relay Process did not find file {_filePath}");
                return;
            }

            var tradeOrders = new List<TradeOrderFrame>();

            using (var reader = File.OpenText(_filePath))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.HasHeaderRecord = true;
                var csvRecords = csv.GetRecords<TradeOrderFrameCsv>().ToList();

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
                Logger.Error($"TradingFileRelayProcess had {_csvToDtoMapper.FailedParseTotal} errors parsing the input CSV file {_filePath}");
            }

            if (!tradeOrders.Any())
            {
                return;
            }

            var sortedTradeOrders = tradeOrders.OrderBy(to => to.StatusChangedOn).ToList();

            foreach (var item in sortedTradeOrders)
            {
                TradeStream.Add(item);
            }
        }

        public override void OnNext(ExchangeFrame value)
        {
        }

        protected override void _TerminateTradingStrategy()
        {
        }
    }
}