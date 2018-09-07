using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Domain.Trades.Orders;
using Domain.Trades.Orders.Interfaces;
using Microsoft.Extensions.Logging;
using Relay.Disk_IO.Interfaces;

namespace Relay.Disk_IO
{
    public class UploadTradeFileProcessor : IUploadTradeFileProcessor
    {
        private readonly ITradeOrderCsvToDtoMapper _csvToDtoMapper;
        private readonly ILogger _logger;

        public UploadTradeFileProcessor(
            ITradeOrderCsvToDtoMapper csvToDtoMapper,
            ILogger<UploadTradeFileProcessor> logger)
        {
            _csvToDtoMapper = csvToDtoMapper ?? throw new ArgumentNullException(nameof(csvToDtoMapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyCollection<TradeOrderFrame> Process(string path)
        {
            if (!File.Exists(path))
            {
                _logger.LogError($"Upload Trade File Monitor did not find file {path}");
                return new List<TradeOrderFrame>();
            }

            var tradeOrders = new List<TradeOrderFrame>();

            using (var reader = File.OpenText(path))
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
                _logger.LogError($"TradingFileRelayProcess had {_csvToDtoMapper.FailedParseTotal} errors parsing the input CSV file {path}");
            }

            if (!tradeOrders.Any())
            {
                return new List<TradeOrderFrame>();
            }

            var sortedTradeOrders = tradeOrders.OrderBy(to => to.StatusChangedOn).ToList();

            return sortedTradeOrders;
        }
    }
}
