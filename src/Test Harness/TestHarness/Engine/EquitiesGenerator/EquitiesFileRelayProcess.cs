using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Domain.Equity.Frames;
using Domain.Equity.Frames.Interfaces;
using Domain.Equity.Streams.Interfaces;
using Domain.Market;
using NLog;
using TestHarness.Engine.EquitiesGenerator.Interfaces;

namespace TestHarness.Engine.EquitiesGenerator
{
    public class EquitiesFileRelayProcess : IEquityDataGenerator
    {
        private readonly string _filePath;
        private readonly ILogger _logger;
        private readonly ISecurityCsvToDtoMapper _securityMapper;

        public EquitiesFileRelayProcess(
            string filePath,
            ILogger logger,
            ISecurityCsvToDtoMapper securityMapper)
        {
            _filePath = filePath ?? string.Empty;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _securityMapper = securityMapper ?? throw new ArgumentNullException(nameof(securityMapper));
        }

        public void InitiateWalk(IStockExchangeStream stream)
        {
            if (string.IsNullOrWhiteSpace(_filePath))
            {
                _logger.Error($"Equities File Relay Process did not find file because the path was empty or null");
                return;
            }

            if (!File.Exists(_filePath))
            {
                _logger.Error($"Equities File Relay Process did not find file {_filePath}");
                return;
            }

            var securities = new List<SecurityTick>();

            var mic = string.Empty;
            var marketName = string.Empty;

            using (var reader = File.OpenText(_filePath))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.HasHeaderRecord = true;

                var csvRecords = csv.GetRecords<SecurityTickCsv>().ToList();

                foreach (var record in csvRecords)
                {
                    if (string.IsNullOrWhiteSpace(mic))
                    {
                        mic = record.MarketIdentifierCode;
                    }

                    if (string.IsNullOrWhiteSpace(marketName))
                    {
                        marketName = record.MarketName;
                    }

                    var mappedRecord = _securityMapper.Map(record);
                    if (mappedRecord != null)
                    {
                        securities.Add(mappedRecord);
                    }
                }
            }

            if (_securityMapper.FailedParseTotal > 0)
            {
                _logger.Error($"EquitiesFileRelayProcess had {_securityMapper.FailedParseTotal} errors parsing the input CSV file {_filePath}");
            }

            if (!securities.Any())
            {
                return;
            }

            var stockExchange = new StockExchange(new Market.MarketId(mic), marketName);
            var exchange = new ExchangeFrame(stockExchange, securities);
            stream.Add(exchange);
        }

        public void TerminateWalk()
        {
            // no op
        }
    }
}
